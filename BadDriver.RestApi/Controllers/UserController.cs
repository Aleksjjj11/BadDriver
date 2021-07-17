using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BadDriver.RestApi.Models;
using BadDriver.RestApi.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BadDriver.RestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "admin")]
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            await using var repository = new ApplicationContext();
            return Json(repository.Users.ToList());
        }

        [HttpPost("token")]
        public IActionResult Token([FromForm] TokenRequest tokenRequest)
        {
            var identity = GetIdentity(tokenRequest.Username, tokenRequest.Password);

            if (identity == null)
                return BadRequest(new { errorText = "Invalid username or password." });

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.Issuer,
                audience: AuthOptions.Audience,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.Lifetime)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };

            return Json(response);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromForm] CreateUserRequest userRequest)
        {
            await using var repository = new ApplicationContext();
            var duplicateUser = await repository.Users.FirstOrDefaultAsync(x => x.Username == userRequest.Username);

            if (duplicateUser != null)
            {
                return Problem("Пользователь с таким логином уже существует");
            }

            var newUser = new User
            {
                Username = userRequest.Username,
                FirstName = userRequest.FirstName,
                LastName = userRequest.LastName,
                Password = userRequest.Password,
                Email = userRequest.Email,
                Role = "user"
            };

            await repository.Users.AddAsync(newUser);
            var countLineChanged = await repository.SaveChangesAsync();

            return countLineChanged > 0
                ? Ok()
                : ValidationProblem();
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("reports")]
        public async Task<IActionResult> GetReports([FromForm] int userId)
        {
            await using var repository = new ApplicationContext();
            var reports = repository.Reports.Where(x => x.UserId == userId).ToList();
            return Json(reports);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("cars")]
        public async Task<IActionResult> GetCars([FromForm] int userId)
        {
            await using var repository = new ApplicationContext();
            var cars = repository.Cars.Where(x => x.UserId == userId).ToList();
            return Json(cars);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("achievements")]
        public async Task<IActionResult> GetAchievements([FromForm] int userId)
        {
            await using var repository = new ApplicationContext();
            var achievements = repository.UserAchievements
                .Where(x => x.UserId == userId)
                .Select(x => x.AchievementId)
                .Select(x => repository.Achievements.FirstOrDefault(y => y.Id == x));

            return Json(achievements.ToList());
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "admin")]
        [HttpPost("achieve")]
        public async Task<IActionResult> Achieve([FromForm] UserAchieveRequest userAchieveRequest)
        {
            await using var repository = new ApplicationContext();

            repository.UserAchievements.Add(new UserAchievements
            {
                UserId = userAchieveRequest.UserId,
                AchievementId = userAchieveRequest.AchievementId
            });

            return await repository.SaveChangesAsync() > 0
                ? Ok()
                : Problem("Не удалось присвоить данное достижение данному пользователю");
        }

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            using var applicationContext = new ApplicationContext();

            var person = applicationContext.Users.SingleOrDefault(x => x.Username == username && x.Password == password);

            if (person == null) 
                return null;

            var claims = new List<Claim>
            {
                new(ClaimsIdentity.DefaultNameClaimType, person.Username),
                new(ClaimsIdentity.DefaultRoleClaimType, person.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            return claimsIdentity;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
    }
}

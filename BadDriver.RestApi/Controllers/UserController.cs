using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BadDriver.RestApi.Jwt;
using BadDriver.RestApi.Models;
using BadDriver.RestApi.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BadDriver.RestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IJwtManager _jwtManager;

        public UserController(IJwtManager jwtManager)
        {
            _jwtManager = jwtManager;
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "admin")]
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            await using var repository = new ApplicationContext();
            return Json(repository.Users.ToList());
        }

        [HttpPost("tokens")]
        public IActionResult Tokens([FromForm] TokensRequest tokensRequest)
        {
            var identity = GetIdentity(tokensRequest.Username, tokensRequest.Password);

            if (identity == null)
                return Problem("Invalid username or password.");

            var tokens = _jwtManager.GenerateTokens(tokensRequest.Username, identity.Claims, DateTime.UtcNow);

            return Json(tokens);
        }

        [HttpPost("access")]
        public IActionResult AccessToken([FromBody] RefreshAccessRequest refreshAccessRequest)
        {
            var tokens  = _jwtManager.Refresh(refreshAccessRequest.RefreshToken, refreshAccessRequest.AccessToken, DateTime.UtcNow);
            return Json(tokens);
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
    }
}

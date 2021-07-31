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
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IJwtManager _jwtManager;

        public UserController(IJwtManager jwtManager)
        {
            _jwtManager = jwtManager;
        }

        [Authorize(Roles = "admin")]
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            await using var repository = new ApplicationContext();
            return Json(repository.Users.ToList());
        }

        [AllowAnonymous]
        [HttpPost("tokens")]
        public IActionResult Tokens([FromBody] TokensRequest tokensRequest)
        {
            var identity = GetIdentity(tokensRequest.Username, tokensRequest.Password);

            if (identity == null)
                return Problem("Invalid username or password.");

            var tokens = _jwtManager.GenerateTokens(tokensRequest.Username, identity.Claims, DateTime.UtcNow);

            return Json(tokens);
        }

        [AllowAnonymous]
        [HttpPost("access")]
        public IActionResult AccessToken([FromBody] RefreshAccessRequest refreshAccessRequest)
        {
            var tokens  = _jwtManager.Refresh(refreshAccessRequest.RefreshToken, refreshAccessRequest.AccessToken, DateTime.UtcNow);
            return Json(tokens);
        }

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

        [HttpGet("about")]
        public async Task<IActionResult> GetInfoAboutUser()
        {
            await using var repository = new ApplicationContext();

            var user = await GetCurrentUser();

            return Json(user);
        }

        [HttpGet("reports")]
        public async Task<IActionResult> GetReports()
        {
            await using var repository = new ApplicationContext();

            var user = await GetCurrentUser();
            var reports = repository.Reports.Where(x => x.UserId == user.Id).ToList();

            return Json(reports);
        }

        [HttpGet("cars")]
        public async Task<IActionResult> GetCars()
        {
            await using var repository = new ApplicationContext();

            var user = await GetCurrentUser();
            var cars = repository.Cars.Where(x => x.UserId == user.Id).ToList();

            return Json(cars);
        }

        [HttpGet("achievements")]
        public async Task<IActionResult> GetAchievements()
        {
            await using var repository = new ApplicationContext();

            var user = await GetCurrentUser();

            var achievements = repository.UserAchievements
                .Where(x => x.UserId == user.Id)
                .Select(x => x.AchievementId)
                .Select(x => repository.Achievements.FirstOrDefault(y => y.Id == x));

            return Json(achievements.ToList());
        }

        [Authorize(Roles = "admin")]
        [HttpPost("achieve")]
        public async Task<IActionResult> Achieve([FromBody] UserAchieveRequest userAchieveRequest)
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

        private async Task<User> GetCurrentUser()
        {
            await using var repository = new ApplicationContext();

            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var username = _jwtManager.DecodeJwtToken(token).Item1.Claims.Single(x => x.Type == ClaimTypes.Name).Value;

            return await repository.Users.SingleAsync(x => x.Username == username);
        }
    }
}

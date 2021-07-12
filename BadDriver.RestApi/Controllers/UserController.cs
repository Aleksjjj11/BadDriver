using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using BadDriver.RestApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BadDriver.RestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        
        [HttpGet]
        public ActionResult GetUsers()
        {
            List<User> localCount;

            using (var db = new ApplicationContext())
            {
                localCount = db.Users.ToList();
            }

            return Json(localCount);
        }

        [HttpPost("/token")]
        public ActionResult Token([FromForm] string username, [FromForm] string password)
        {
            var identity = GetIdentity(username, password);

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
        [HttpGet("/auth")]
        public ActionResult AuthorizedMethodTest()
        {
            return Json(new
            {
                text = "These all good"
            });
        }

        [Authorize(Roles = "admin", AuthenticationSchemes = "Bearer")]
        [HttpGet("/authAdmin")]
        public ActionResult AuthorizedAdminMethodTest()
        {
            return Json(new
            {
                text = "These all good"
            });
        }

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            using var db = new ApplicationContext();

            var person = db.Users.SingleOrDefault(x => x.Username == username && x.Password == password);

            if (person == null) 
                return null;

            var claims = new List<Claim>
            {
                new(ClaimsIdentity.DefaultNameClaimType, person.Username),
                new(ClaimsIdentity.DefaultRoleClaimType, person.Role.ToString())
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

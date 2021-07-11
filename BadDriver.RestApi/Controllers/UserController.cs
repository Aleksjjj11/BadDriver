using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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
        private readonly List<User> _people = new()
        {
            new() {Username= "admin", Password="12345", Role = UserRole.Admin },
            new() { Username= "qwerty", Password="55555", Role = UserRole.User }
        };

        [HttpGet]
        public ActionResult GetUsers()
        {
            return Json(_people);
        }

        [HttpPost("/token")]
        public ActionResult Token(string username, string password)
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

        [Authorize(Roles = "Admin", AuthenticationSchemes = "Bearer")]
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
            var person = _people.FirstOrDefault(x => x.Username == username && x.Password == password);

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
    }
}

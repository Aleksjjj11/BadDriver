using System.Linq;
using BadDriver.RestApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BadDriver.RestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AchievementController : Controller
    {
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "admin, user")]
        [HttpGet("achievements")]
        public IActionResult GetAchievements()
        {
            using var db = new ApplicationContext();
            return Json(db.Achievements.ToList());
        }
    }
}

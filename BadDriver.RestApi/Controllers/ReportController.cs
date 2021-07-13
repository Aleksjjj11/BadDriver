using System.Linq;
using BadDriver.RestApi.Models;
using BadDriver.RestApi.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BadDriver.RestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : Controller
    {
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "admin")]
        [HttpGet("reports")]
        public IActionResult GetReports()
        {
            using var db = new ApplicationContext();
            return Json(db.Reports.ToList());
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("create")]
        public IActionResult CreateReport([FromForm] CreateReportRequest reportRequest)
        {
            var newReport = new Report
            {
                CarId = reportRequest.CarId,
                Description = reportRequest.Description,
                UserId = reportRequest.UserId,
                ImageUrl1 = reportRequest.ImageUrl1,
                ImageUrl2 = reportRequest.ImageUrl2,
                ImageUrl3 = reportRequest.ImageUrl3
            };

            using var repository = new ApplicationContext();
            repository.Reports.Add(newReport);
            
            return repository.SaveChanges() > 0
                ? Ok()
                : ValidationProblem();
        }
    }
}

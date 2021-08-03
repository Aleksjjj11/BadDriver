using System.Linq;
using System.Threading.Tasks;
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
        public async Task<IActionResult> CreateReport([FromBody] CreateReportRequest reportRequest)
        {
            await using var repository = new ApplicationContext();

            var reportRequestCar = reportRequest.Car;
            var car = repository.Cars.FirstOrDefault(x => x.CountryCode == reportRequestCar.CountryCode
                                                          && x.Number == reportRequestCar.Number
                                                          && x.RegionCode == reportRequestCar.RegionCode);

            if (car == null)
            {
                repository.Cars.Add(reportRequestCar);
                await repository.SaveChangesAsync();
            }
            else
            {
                reportRequestCar.Id = car.Id;
            }

            var newReport = new Report
            {
                CarId = reportRequestCar.Id,
                Description = reportRequest.Description,
                UserId = reportRequest.UserId,
                ImageUrl1 = reportRequest.ImageUrl1,
                ImageUrl2 = reportRequest.ImageUrl2,
                ImageUrl3 = reportRequest.ImageUrl3
            };

            repository.Reports.Add(newReport);
            
            return await repository.SaveChangesAsync() > 0
                ? Ok()
                : ValidationProblem();
        }
    }
}

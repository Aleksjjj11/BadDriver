using System.Threading.Tasks;
using BadDriver.RestApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BadDriver.RestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class CarController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> GetCar(int id)
        {
            var repository = new ApplicationContext();
            var car = await repository.Cars.SingleAsync(x => x.Id == id);

            return Json(new
            {
                id = car.Id,
                number = car.Number,
                region_code = car.RegionCode,
                country_code = car.CountryCode
            });
        }
    }
}
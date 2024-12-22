using ClassConstructorGeneratorTest.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClassConstructorGeneratorTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public partial class WeatherAlertController : ControllerBase
    {
        private readonly ILogger<WeatherAlertController> _logger;
        private readonly IWeatherAlertService _weatherAlertService;

        [HttpGet(Name = "GetWeatherAlert")]
        public IEnumerable<WeatherAlert> Get()
        {
            try
            {
                _logger.LogDebug("Starting to retrieve the weather alert list.");
                return _weatherAlertService.List(5);
            }
            finally
            {
                _logger.LogInformation("Weather alert list has been retrieved.");
            }
        }
    }
}
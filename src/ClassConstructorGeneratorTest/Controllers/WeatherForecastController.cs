using ClassConstructorGeneratorTest.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClassConstructorGeneratorTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public partial class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeatherForecastService _weatherForecastService;

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            try
            {
                _logger.LogDebug("Starting to retrieve the weather forecast list.");
                return _weatherForecastService.List(5);
            }
            finally
            {
                _logger.LogInformation("Weather forecast list has been retrieved.");
            }
        }
    }
}
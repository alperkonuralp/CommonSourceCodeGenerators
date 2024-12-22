namespace ClassConstructorGeneratorTest.Services
{
    public partial class WeatherForecastService : IWeatherForecastService
    {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        private readonly ILogger<WeatherForecastService> _logger;

        public WeatherForecast[] List(int dayCount)
        {
            _logger.LogInformation("List method started with dayCount: {DayCount}", dayCount);

            var forecasts = Enumerable
                .Range(1, dayCount)
                .Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
                .ToArray();

            _logger.LogInformation("List method completed with {Count} forecasts generated.", forecasts.Length);
            return forecasts;
        }
    }
}
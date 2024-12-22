namespace ClassConstructorGeneratorTest.Services
{
    [IgnoreConstructorGenerator]
    public partial class WeatherAlertService : IWeatherAlertService
    {
        private static readonly string[] AlertTypes =
        {
            "Storm Warning", "Heat Advisory", "Flood Watch", "Tornado Warning", "Snow Advisory"
        };

        private readonly ILogger<WeatherAlertService> _logger;

        public WeatherAlertService(ILogger<WeatherAlertService> logger)
        {
            _logger = logger;
        }

        public WeatherAlert[] List(int alertCount)
        {
            _logger.LogInformation("GetAlerts method started with alertCount: {AlertCount}", alertCount);

            var alerts = Enumerable
                .Range(1, alertCount)
                .Select(index => new WeatherAlert
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    AlertType = AlertTypes[Random.Shared.Next(AlertTypes.Length)],
                    Message = $"This is a {AlertTypes[Random.Shared.Next(AlertTypes.Length)]} for your area."
                })
                .ToArray();

            _logger.LogInformation("GetAlerts method completed with {Count} alerts generated.", alerts.Length);
            return alerts;
        }
    }
}
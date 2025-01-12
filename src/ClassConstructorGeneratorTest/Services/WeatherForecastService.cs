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

    public interface IRepository<T>
    {
        T GetById(int id);

        List<T> List();
    }

    public class Repository<T> : IRepository<T>
    {
        private readonly ILogger<Repository<T>> _logger;

        public Repository(ILogger<Repository<T>> logger)
        {
            _logger = logger;
        }

        public List<T> List()
        {
            _logger.LogInformation("List method started.");
            var items = new List<T>();
            _logger.LogInformation("List method completed with {Count} items generated.", items.Count);
            return items;
        }

        public T GetById(int id)
        {
            _logger.LogInformation("GetById method started with id: {Id}", id);
            var item = default(T);
            _logger.LogInformation("GetById method completed.");
            return item;
        }
    }

    public record Data(int Id, string Name);

    public partial class DataRepository : Repository<Data>
    {
        private readonly IWeatherForecastService _weatherForecastService;
    }
}
using System.Data;

namespace ExportInterfaceGeneratorTest.Services
{
    public interface IService;

    public partial interface IWeatherForecastService : IService;

    public class WeatherForecastService : IWeatherForecastService
    {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        /// <summary>
        /// Integer Property
        /// </summary>
        public int IntProp { get; set; }

        /// <summary>
        /// String Property
        /// </summary>
        public string StringProp { get; private set; }

        /// <summary>
        /// DateTimeOffset Property
        /// </summary>
        public DateTimeOffset DateTimeOffsetProp { private get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name => "Deneme";

        /// <summary>
        /// Get Weather Forecast
        /// </summary>
        /// <returns> List of forecasts </returns>
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [IgnoreToInterfaceGeneration]
        public IEnumerable<WeatherForecast> Get2()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        public WeatherForecast GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        internal WeatherForecast GetById2(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Add(WeatherForecast weatherForecast)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(WeatherForecast weatherForecast)
        {
            throw new NotImplementedException();
        }

        public void Update(ref WeatherForecast weatherForecast)
        {
            throw new NotImplementedException();
        }

        public bool IsExist(Guid id, out WeatherForecast weatherForecast)
        {
            throw new NotImplementedException();
        }

        public void DynamicParameters(params WeatherForecast[] weatherForecast)
        {
            throw new NotImplementedException();
        }

        public void DynamicParameters2(params WeatherForecast[] weatherForecast)
            => throw new NotImplementedException();

        public Task AsyncDemo1()
        {
            throw new NotImplementedException();
        }

        public Task<bool> AsyncDemo2()
        {
            throw new NotImplementedException();
        }

        public Task<DataSet> AsyncDemo3()
        {
            throw new NotImplementedException();
        }
    }
}
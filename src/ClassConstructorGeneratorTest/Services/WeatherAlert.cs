namespace ClassConstructorGeneratorTest.Services
{
    public class WeatherAlert
    {
        public DateOnly Date { get; init; }
        public required string AlertType { get; init; }
        public required string Message { get; init; }
    }
}
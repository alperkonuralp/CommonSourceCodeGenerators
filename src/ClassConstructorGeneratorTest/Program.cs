using ClassConstructorGeneratorTest.Services;

namespace ClassConstructorGeneratorTest
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();
            builder.Services.AddScoped<IWeatherAlertService, WeatherAlertService>();

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi("/openapi/v1/openapi.json");
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/openapi/v1/openapi.json", "My API V1"));
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
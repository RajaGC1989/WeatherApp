using WeatherApp.Domain.Models;

namespace WeatherApp.Application.Interfaces
{
    public interface IWeatherService
    {
        Task<Weather> GetWeatherForecastAsync(string city);
    }
}

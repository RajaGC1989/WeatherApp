using WeatherApp.Application.DTO;
using WeatherApp.Domain.Models;

namespace WeatherApp.Application.Interfaces
{
    public interface IWeatherService
    {
        Task<ApiResponse<Weather>> GetWeatherForecastAsync(string city);
    }
}

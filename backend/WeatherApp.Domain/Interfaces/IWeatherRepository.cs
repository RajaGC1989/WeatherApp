using WeatherApp.Domain.Models;

namespace WeatherApp.Domain.Interfaces
{
    public interface IWeatherRepository
    {
        Task<List<Weather>> GetAllWeatherAsync();
        Task<Weather> GetWeatherByCityAsync(string city);
        Task<Weather> AddWeatherAsync(Weather weather);
        Task<Weather> UpdateWeatherAsync(Weather weather);
        Task<bool> DeleteWeatherAsync(string city);
    }
}

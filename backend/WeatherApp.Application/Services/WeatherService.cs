using Microsoft.Extensions.Options;
using System.Text.Json;
using WeatherApp.Domain.Models;

namespace WeatherApp.Application.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly WeatherApiSettings _apiSettings;

        public WeatherService(HttpClient httpClient, IOptions<WeatherApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
        }

        public async Task<Weather> GetWeatherForecastAsync(string city)
        {
            //https://api.openweathermap.org/data/2.5/weather?q=chennai&appid=2b6260d38ef4ae8da25c6658bb6cac95&units=metric
            var url = $"{_apiSettings.BaseUrl}?q={city}&appid={_apiSettings.ApiKey}&unit={_apiSettings.Unit}";
            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var weatherApiResponse = JsonSerializer.Deserialize<WeatherApiResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (weatherApiResponse == null) return null;

            // Map to your DTO
            return new Weather
            {
                CityName = weatherApiResponse.Name,
                Temperature = weatherApiResponse.Main.Temp,
                WeatherCondition = weatherApiResponse.Weather.FirstOrDefault()?.Main ?? "Unknown",
                LastUpdated = DateTimeOffset.FromUnixTimeSeconds(weatherApiResponse.Dt).UtcDateTime
            };
        }
    }
}

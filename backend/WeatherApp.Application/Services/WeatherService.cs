using Microsoft.Extensions.Options;
using System.Text.Json;
using WeatherApp.Application.Interfaces;
using WeatherApp.Domain.Models;

namespace WeatherApp.Application.Services
{
    public class WeatherService: IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly WeatherApiSettings _apiSettings;

        private readonly string _invalidJsonResponse = "Invalid JSON";

        public WeatherService(HttpClient httpClient, IOptions<WeatherApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
        }

        public WeatherService()
        {
            
        }

        public async Task<Weather> GetWeatherForecastAsync(string city)
        {
            var url = $"{_apiSettings.BaseUrl}?q={city}&appid={_apiSettings.ApiKey}&unit={_apiSettings.Unit}";
            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            WeatherApiResponse? weatherApiResponse = null;

            if (!string.IsNullOrWhiteSpace(content) && content != _invalidJsonResponse)
            {
                weatherApiResponse = JsonSerializer.Deserialize<WeatherApiResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }            

            if (weatherApiResponse == null) return null;

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

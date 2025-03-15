using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using WeatherApp.Application.DTO;
using WeatherApp.Application.Interfaces;
using WeatherApp.Domain.Models;

namespace WeatherApp.Application.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly WeatherApiSettings _apiSettings;
        private readonly string _invalidJsonResponse = "Invalid JSON";

        public WeatherService()
        {
        }

        public WeatherService(HttpClient httpClient, IOptions<WeatherApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
        }

        public async Task<ApiResponse<Weather>> GetWeatherForecastAsync(string city)
        {
            try
            {
                var url = $"{_apiSettings.BaseUrl}?q={city}&appid={_apiSettings.ApiKey}&unit={_apiSettings.Unit}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<Weather>
                    {
                        StatusCode = response.StatusCode,
                        Message = $"Failed to fetch weather data. Status: {response.StatusCode}",
                        Data = null
                    };
                }

                var content = await response.Content.ReadAsStringAsync();
                WeatherApiResponse? weatherApiResponse = null;

                if (!string.IsNullOrWhiteSpace(content) && content != _invalidJsonResponse)
                {
                    weatherApiResponse = JsonSerializer.Deserialize<WeatherApiResponse>(
                        content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                if (weatherApiResponse == null)
                {
                    return new ApiResponse<Weather>
                    {
                        StatusCode = HttpStatusCode.NoContent,
                        Message = "Weather data not available.",
                        Data = null
                    };
                }

                var weather = new Weather
                {
                    CityName = weatherApiResponse.Name,
                    Temperature = weatherApiResponse.Main.Temp,
                    WeatherCondition = weatherApiResponse.Weather.FirstOrDefault()?.Main ?? "Unknown",
                    LastUpdated = DateTimeOffset.FromUnixTimeSeconds(weatherApiResponse.Dt).UtcDateTime
                };

                return new ApiResponse<Weather>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Success",
                    Data = weather
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Weather>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                };
            }
        }
    }
}

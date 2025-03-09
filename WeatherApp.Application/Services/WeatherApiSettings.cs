namespace WeatherApp.Application.Services
{
    public class WeatherApiSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;

        public string Unit { get; set; } = string.Empty;
    }
}

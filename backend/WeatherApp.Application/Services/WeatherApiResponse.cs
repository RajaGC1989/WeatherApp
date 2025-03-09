namespace WeatherApp.Application.Services
{
    public class WeatherApiResponse
    {
        public string Name { get; set; } = string.Empty;
        public Main Main { get; set; } = new Main();
        public List<WeatherCondition> Weather { get; set; } = new List<WeatherCondition>();
        public long Dt { get; set; }
    }

    public class Main
    {
        public decimal Temp { get; set; }
    }

    public class WeatherCondition
    {
        public string Main { get; set; } = string.Empty;
    }
}

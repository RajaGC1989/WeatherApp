namespace WeatherApp.Domain.Models
{
    public class Weather
    {
        public int Id { get; set; }
        public string CityName { get; set; }
        public decimal Temperature { get; set; }
        public string WeatherCondition { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}

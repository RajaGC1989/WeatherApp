using MediatR;
using WeatherApp.Domain.Models;

namespace WeatherApp.Application.CQRS.Queries
{
    public class GetWeatherByCity:IRequest<Weather>
    {
        public GetWeatherByCity(string city)
        {
            City = city;
        }

        public string City { get; set; }
    }
}

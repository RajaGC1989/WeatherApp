using MediatR;
using WeatherApp.Domain.Models;

namespace WeatherApp.Application.CQRS.Commands
{
    public class CreateWeatherCommand : IRequest<Weather>
    {
        public Weather Weather { get; set; }
        public CreateWeatherCommand(Weather weather)
        {
            Weather = weather;
        }
    }
}

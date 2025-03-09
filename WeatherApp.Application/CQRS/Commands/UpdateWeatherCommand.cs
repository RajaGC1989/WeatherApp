using MediatR;
using WeatherApp.Domain.Models;

namespace WeatherApp.Application.CQRS.Commands
{
    public class UpdateWeatherCommand:IRequest<Weather>
    {
        public Weather Weather { get; set; }

        public UpdateWeatherCommand(Weather weather)
        {
            Weather = weather;
        }
    }
}

using MediatR;

namespace WeatherApp.Application.CQRS.Commands
{
    public class DeleteWeatherCommand : IRequest<bool>
    {
        public DeleteWeatherCommand(string city)
        {
            City = city;
        }
        public string City { get; set; }
    }
}

using MediatR;
using WeatherApp.Application.CQRS.Commands;
using WeatherApp.Domain.Interfaces;
using WeatherApp.Domain.Models;

namespace WeatherApp.Application.CQRS.Handlers.CommandHandlers
{
    public class CreateWeatherHandler : IRequestHandler<CreateWeatherCommand, Weather>
    {
        private readonly IWeatherRepository _repository;

        public CreateWeatherHandler(IWeatherRepository repository)
        {
            _repository = repository;
        }

        public Task<Weather> Handle(CreateWeatherCommand request, CancellationToken cancellationToken)
        {
           return _repository.AddWeatherAsync(request.Weather);
        }
    }
}

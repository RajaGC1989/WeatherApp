using MediatR;
using WeatherApp.Application.CQRS.Commands;
using WeatherApp.Domain.Interfaces;
using WeatherApp.Domain.Models;

namespace WeatherApp.Application.CQRS.Handlers.CommandHandlers
{
    public class UpdateWeatherHandler : IRequestHandler<UpdateWeatherCommand, Weather>
    {
        private readonly IWeatherRepository _repository;

        public UpdateWeatherHandler(IWeatherRepository repository)
        {
            _repository = repository;
        }

        public Task<Weather> Handle(UpdateWeatherCommand request, CancellationToken cancellationToken)
        {
            return _repository.UpdateWeatherAsync(request.Weather);
        }
    }
}

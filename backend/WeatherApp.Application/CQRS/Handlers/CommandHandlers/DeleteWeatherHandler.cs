using MediatR;
using WeatherApp.Application.CQRS.Commands;
using WeatherApp.Domain.Interfaces;

namespace WeatherApp.Application.CQRS.Handlers.CommandHandlers
{
    public class DeleteWeatherHandler : IRequestHandler<DeleteWeatherCommand, bool>
    {
        private readonly IWeatherRepository _repository;

        public DeleteWeatherHandler(IWeatherRepository repository)
        {
            _repository = repository;
        }
        public async Task<bool> Handle(DeleteWeatherCommand request, CancellationToken cancellationToken)
        {
            return await _repository.DeleteWeatherAsync(request.City);
        }
    }
}

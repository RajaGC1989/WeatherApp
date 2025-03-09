using MediatR;
using WeatherApp.Application.CQRS.Queries;
using WeatherApp.Domain.Interfaces;
using WeatherApp.Domain.Models;

namespace WeatherApp.Application.CQRS.Handlers.QueryHandlers
{
    public class GetWeatherByCityHandler : IRequestHandler<GetWeatherByCity, Weather>
    {
        private readonly IWeatherRepository _repository;

        public GetWeatherByCityHandler(IWeatherRepository repository)
        {
            _repository = repository;
        }

        public Task<Weather> Handle(GetWeatherByCity request, CancellationToken cancellationToken)
        {
            return _repository.GetWeatherByCityAsync(request.City);
        }
    }
}

using MediatR;
using WeatherApp.Application.CQRS.Queries;
using WeatherApp.Domain.Interfaces;
using WeatherApp.Domain.Models;

namespace WeatherApp.Application.CQRS.Handlers.QueryHandlers
{
    public class GetAllWeatherHandler : IRequestHandler<GetAllWeatherQuery, List<Weather>>
    {
        private readonly IWeatherRepository _repository;
        public GetAllWeatherHandler(IWeatherRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Weather>> Handle(GetAllWeatherQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAllWeatherAsync();
        }
    }
}

using MediatR;
using WeatherApp.Domain.Models;

namespace WeatherApp.Application.CQRS.Queries
{
    public class GetAllWeatherQuery:IRequest<List<Weather>>
    {
    }
}

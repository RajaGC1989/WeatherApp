using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using WeatherApp.Application.CQRS.Commands;
using WeatherApp.Application.CQRS.Queries;
using WeatherApp.Application.Interfaces;
using WeatherApp.Application.Services;
using WeatherApp.Domain.Models;

namespace WeatherApp.API.Controllers
{
    [Route("api/weather")]
    [ApiController]
    [Authorize]
    public class WeatherController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IWeatherService _weatherService;

        public WeatherController(IMediator mediator, WeatherService weatherService)
        {
            _mediator = mediator;
            _weatherService = weatherService;
        }

        public WeatherController()
        {
            
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWeather()
        {
            var result = await _mediator.Send(new GetAllWeatherQuery());
            return Ok(result);
        }

        [HttpGet("{city}")]
        public async Task<IActionResult> GetWeatherByCity(string city)
        {
            var result = await _mediator.Send(new GetWeatherByCity(city));
            if (result == null)
                return NotFound("Please add the city");

            return Ok(result);
        }

        [HttpGet("fetch/{city}")]
        public async Task<IActionResult> FetchWeatherFromAPI(string city)
        {
            var weather = await _weatherService.GetWeatherForecastAsync(city);
            if (weather == null)
                return NotFound();

            await _mediator.Send(new CreateWeatherCommand(weather));
            return Ok(weather);
        }

        [HttpDelete("{city}")]
        public async Task<IActionResult> DeleteWeather(string city)
        {
            var success = await _mediator.Send(new DeleteWeatherCommand(city));
            if (!success)
                return NotFound("City not found");

            return NoContent();
        }

        [HttpPut("{city}")]
        public async Task<IActionResult> UpdateWeather(string city)
        {
            var weather = await _weatherService.GetWeatherForecastAsync(city);

            if (weather == null)
                return NotFound("City not found!");

            var updatedWeather = await _mediator.Send(new UpdateWeatherCommand(weather));

            if (updatedWeather == null)
                return NotFound("City not found");

            return Ok(updatedWeather);
        }
    }
}

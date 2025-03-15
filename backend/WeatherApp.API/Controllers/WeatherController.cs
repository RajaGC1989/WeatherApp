using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WeatherApp.Application.CQRS.Commands;
using WeatherApp.Application.CQRS.Queries;
using WeatherApp.Application.DTO;
using WeatherApp.Application.Interfaces;
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

        public WeatherController(IMediator mediator, IWeatherService weatherService)
        {
            _mediator = mediator;
            _weatherService = weatherService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWeather()
        {
            try
            {
                var result = await _mediator.Send(new GetAllWeatherQuery());
                return Ok(new ApiResponse<IEnumerable<Weather>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Weather data retrieved successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = $"Error retrieving weather data: {ex.Message}",
                    Data = null
                });
            }
        }

        [HttpGet("{city}")]
        public async Task<IActionResult> GetWeatherByCity(string city)
        {
            try
            {
                var result = await _mediator.Send(new GetWeatherByCity(city));

                if (result == null)
                    return NotFound(new ApiResponse<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "City not found.",
                        Data = null
                    });

                return Ok(new ApiResponse<Weather>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Weather data retrieved successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = $"Error retrieving weather data: {ex.Message}",
                    Data = null
                });
            }
        }

        [HttpGet("fetch/{city}")]
        public async Task<IActionResult> FetchWeatherFromAPI(string city)
        {
            try
            {
                var weatherResponse = await _weatherService.GetWeatherForecastAsync(city);

                if (weatherResponse == null || weatherResponse.Data == null)
                    return NotFound(new ApiResponse<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "Weather data not found from external API.",
                        Data = null
                    });

                await _mediator.Send(new CreateWeatherCommand(weatherResponse.Data));

                return Ok(new ApiResponse<Weather>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Weather data fetched and saved successfully.",
                    Data = weatherResponse.Data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = $"Error fetching weather data: {ex.Message}",
                    Data = null
                });
            }
        }

        [HttpDelete("{city}")]
        public async Task<IActionResult> DeleteWeather(string city)
        {
            try
            {
                var success = await _mediator.Send(new DeleteWeatherCommand(city));

                if (!success)
                    return NotFound(new ApiResponse<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "City not found.",
                        Data = null
                    });

                return Ok(new ApiResponse<string>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Weather data deleted successfully.",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = $"Error deleting weather data: {ex.Message}",
                    Data = null
                });
            }
        }

        [HttpPut("{city}")]
        public async Task<IActionResult> UpdateWeather(string city)
        {
            try
            {
                var weatherResponse = await _weatherService.GetWeatherForecastAsync(city);

                if (weatherResponse == null || weatherResponse.Data == null)
                    return NotFound(new ApiResponse<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "City not found in external API.",
                        Data = null
                    });

                var updatedWeather = await _mediator.Send(new UpdateWeatherCommand(weatherResponse.Data));

                if (updatedWeather == null)
                    return NotFound(new ApiResponse<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "Failed to update city weather.",
                        Data = null
                    });

                return Ok(new ApiResponse<Weather>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Weather data updated successfully.",
                    Data = updatedWeather
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = $"Error updating weather data: {ex.Message}",
                    Data = null
                });
            }
        }
    }
}

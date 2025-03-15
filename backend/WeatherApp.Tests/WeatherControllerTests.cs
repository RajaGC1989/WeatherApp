using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using WeatherApp.API.Controllers;
using WeatherApp.Application.CQRS.Commands;
using WeatherApp.Application.CQRS.Queries;
using WeatherApp.Application.DTO;
using WeatherApp.Application.Interfaces;
using WeatherApp.Domain.Models;
using Xunit;

namespace WeatherApp.UnitTests
{
    public class WeatherControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IWeatherService> _weatherServiceMock;
        private readonly WeatherController _controller;

        public WeatherControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _weatherServiceMock = new Mock<IWeatherService>();
            _controller = new WeatherController(_mediatorMock.Object, _weatherServiceMock.Object);
        }

        [Fact]
        public async Task GetAllWeather_ReturnsOk_WithWeatherList()
        {
            var mockWeatherList = new List<Weather>
            {
                new Weather { Id = 1, CityName = "London", Temperature = 20.5M, WeatherCondition = "Sunny" }
            };

            var apiResponse = new ApiResponse<IEnumerable<Weather>>
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Weather data retrieved successfully.",
                Data = mockWeatherList
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllWeatherQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mockWeatherList);

            var result = await _controller.GetAllWeather();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<Weather>>>(okResult.Value);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Weather data retrieved successfully.", response.Message);
            Assert.Equal(mockWeatherList, response.Data);
        }

        [Fact]
        public async Task GetWeatherByCity_ReturnsOk_WhenWeatherExists()
        {
            var city = "London";
            var mockWeather = new Weather { Id = 1, CityName = city, Temperature = 20.5M, WeatherCondition = "Sunny" };

            var apiResponse = new ApiResponse<Weather>
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Weather data retrieved successfully.",
                Data = mockWeather
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetWeatherByCity>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mockWeather);

            var result = await _controller.GetWeatherByCity(city);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<Weather>>(okResult.Value);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Weather data retrieved successfully.", response.Message);
            Assert.Equal(mockWeather, response.Data);
        }

        [Fact]
        public async Task FetchWeatherFromAPI_ReturnsOk_WhenWeatherIsFetched()
        {
            var apiResponse = new ApiResponse<Weather>
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Weather data fetched and saved successfully.",
                Data = new Weather
                {
                    CityName = "Paris",
                    Temperature = 18.5M,
                    WeatherCondition = "Cloudy",
                    LastUpdated = DateTime.Now
                }
            };

            var city = "Paris";

            _weatherServiceMock.Setup(s => s.GetWeatherForecastAsync(city))
                               .ReturnsAsync(apiResponse);

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateWeatherCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(apiResponse.Data);

            var result = await _controller.FetchWeatherFromAPI(city);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<Weather>>(okResult.Value);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Weather data fetched and saved successfully.", response.Message);
            Assert.Equal(apiResponse.Data, response.Data);
        }

        [Fact]
        public async Task FetchWeatherFromAPI_ReturnsNotFound_WhenWeatherIsNull()
        {
            var city = "Unknown";
            _weatherServiceMock.Setup(s => s.GetWeatherForecastAsync(city))
                               .ReturnsAsync(new ApiResponse<Weather>
                               {
                                   StatusCode = HttpStatusCode.NotFound,
                                   Message = "Weather data not found from external API.",
                                   Data = null
                               });

            var result = await _controller.FetchWeatherFromAPI(city);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(notFoundResult.Value);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("Weather data not found from external API.", response.Message);
            Assert.Null(response.Data);
        }

        [Fact]
        public async Task DeleteWeather_ReturnsOk_WhenCityExists()
        {
            var city = "New York";

            var apiResponse = new ApiResponse<string>
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Weather data deleted successfully.",
                Data = null
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteWeatherCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(true);

            var result = await _controller.DeleteWeather(city);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Weather data deleted successfully.", response.Message);
            Assert.Null(response.Data);
        }

        [Fact]
        public async Task UpdateWeather_ReturnsOk_WhenWeatherIsUpdated()
        {
            var apiResponse = new ApiResponse<Weather>
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Weather data updated successfully.",
                Data = new Weather
                {
                    CityName = "Tokyo",
                    Temperature = 18.5M,
                    WeatherCondition = "Cloudy",
                    LastUpdated = DateTime.Now
                }
            };

            var city = "Tokyo";

            _weatherServiceMock.Setup(s => s.GetWeatherForecastAsync(city))
                               .ReturnsAsync(apiResponse);

            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateWeatherCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(apiResponse.Data);

            var result = await _controller.UpdateWeather(city);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<Weather>>(okResult.Value);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Weather data updated successfully.", response.Message);
            Assert.Equal(apiResponse.Data, response.Data);
        }
    }
}

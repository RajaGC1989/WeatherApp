using Xunit;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using WeatherApp.API.Controllers;
using WeatherApp.Application.CQRS.Commands;
using WeatherApp.Application.CQRS.Queries;
using WeatherApp.Application.Services;
using System.Collections.Generic;
using System.Threading;
using WeatherApp.Domain.Models;

public class WeatherControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<WeatherService> _weatherServiceMock;
    private readonly WeatherController _controller;

    public WeatherControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _weatherServiceMock = new Mock<WeatherService>();
        _controller = new WeatherController(_mediatorMock.Object, _weatherServiceMock.Object);
    }

    [Fact]
    public async Task GetAllWeather_ReturnsOk_WithWeatherList()
    {
        // Arrange
        var mockWeatherList = new List<Weather>
        {
            new Weather { Id = 1, CityName = "London", Temperature = 20.5M, WeatherCondition = "Sunny" }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllWeatherQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(mockWeatherList);

        // Act
        var result = await _controller.GetAllWeather();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockWeatherList, okResult.Value);
    }

    [Fact]
    public async Task GetWeatherByCity_ReturnsOk_WhenWeatherExists()
    {
        // Arrange
        var city = "London";
        var mockWeather = new Weather { Id = 1, CityName = city, Temperature = 20.5M, WeatherCondition = "Sunny" };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetWeatherByCity>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(mockWeather);

        // Act
        var result = await _controller.GetWeatherByCity(city);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockWeather, okResult.Value);
    }

    [Fact]
    public async Task GetWeatherByCity_ReturnsNotFound_WhenWeatherDoesNotExist()
    {
        // Arrange
        var city = "Unknown";
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetWeatherByCity>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Weather)null);

        // Act
        var result = await _controller.GetWeatherByCity(city);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Please add the city", notFoundResult.Value);
    }

    [Fact]
    public async Task FetchWeatherFromAPI_ReturnsOk_WhenWeatherIsFetched()
    {
        // Arrange
        var city = "Paris";
        var mockWeather = new Weather { Id = 2, CityName = city, Temperature = 18.5M, WeatherCondition = "Cloudy" };

        _weatherServiceMock.Setup(s => s.GetWeatherForecastAsync(city))
                   .ReturnsAsync(mockWeather);

        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateWeatherCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(mockWeather);

        // Act
        var result = await _controller.FetchWeatherFromAPI(city);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockWeather, okResult.Value);
    }

    [Fact]
    public async Task FetchWeatherFromAPI_ReturnsNotFound_WhenWeatherIsNull()
    {
        // Arrange
        var city = "Unknown";
        _weatherServiceMock.Setup(s => s.GetWeatherForecastAsync(city))
                           .ReturnsAsync((Weather)null);

        // Act
        var result = await _controller.FetchWeatherFromAPI(city);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteWeather_ReturnsNoContent_WhenCityExists()
    {
        // Arrange
        var city = "New York";
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteWeatherCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteWeather(city);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteWeather_ReturnsNotFound_WhenCityDoesNotExist()
    {
        // Arrange
        var city = "Unknown";
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteWeatherCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteWeather(city);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("City not found", notFoundResult.Value);
    }

    [Fact]
    public async Task UpdateWeather_ReturnsOk_WhenWeatherIsUpdated()
    {
        // Arrange
        var city = "Tokyo";
        var mockWeather = new Weather { Id = 3, CityName = city, Temperature = 22.0M, WeatherCondition = "Rainy" };

        _weatherServiceMock.Setup(s => s.GetWeatherForecastAsync(city))
                           .ReturnsAsync(mockWeather);

        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateWeatherCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(mockWeather);

        // Act
        var result = await _controller.UpdateWeather(city);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockWeather, okResult.Value);
    }

    [Fact]
    public async Task UpdateWeather_ReturnsNotFound_WhenWeatherIsNull()
    {
        // Arrange
        var city = "Unknown";
        _weatherServiceMock.Setup(s => s.GetWeatherForecastAsync(city))
                           .ReturnsAsync((Weather)null);

        // Act
        var result = await _controller.UpdateWeather(city);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("City not found!", notFoundResult.Value);
    }
}

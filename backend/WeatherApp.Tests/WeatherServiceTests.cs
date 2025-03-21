﻿using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using WeatherApp.Application.Services;
using Xunit;

namespace WeatherApp.UnitTests
{
    public class WeatherServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly Mock<IOptions<WeatherApiSettings>> _apiSettingsMock;
        private readonly WeatherService _weatherService;

        public WeatherServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _apiSettingsMock = new Mock<IOptions<WeatherApiSettings>>();
            _apiSettingsMock.Setup(x => x.Value).Returns(new WeatherApiSettings
            {
                BaseUrl = "http://api.weather.com",
                ApiKey = "testapikey",
                Unit = "metric"
            });

            _weatherService = new WeatherService(_httpClient, _apiSettingsMock.Object);
        }

        [Fact]
        public async Task GetWeatherForecastAsync_ReturnsWeather_WhenApiResponseIsValid()
        {
            var city = "London";
            var apiResponse = new WeatherApiResponse
            {
                Name = "London",
                Main = new Main { Temp = 15.5m },
                Weather = new List<WeatherCondition> { new WeatherCondition { Main = "Clear" } },
                Dt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };
            var responseContent = JsonSerializer.Serialize(apiResponse);
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseContent)
            };

            _httpMessageHandlerMock
             .Protected()
             .Setup<Task<HttpResponseMessage>>(
                 "SendAsync",
                 ItExpr.IsAny<HttpRequestMessage>(),
                 ItExpr.IsAny<CancellationToken>()
             )
             .ReturnsAsync(responseMessage);

            var result = await _weatherService.GetWeatherForecastAsync(city);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("London", result.Data.CityName);
            Assert.Equal(15.5m, result.Data.Temperature);
            Assert.Equal("Clear", result.Data.WeatherCondition);
        }

        [Fact]
        public async Task GetWeatherForecastAsync_ReturnsNull_WhenApiResponseIsInvalid()
        {
            var city = "London";
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Invalid JSON")
            };

            _httpMessageHandlerMock
             .Protected()
             .Setup<Task<HttpResponseMessage>>(
                 "SendAsync",
                 ItExpr.IsAny<HttpRequestMessage>(),
                 ItExpr.IsAny<CancellationToken>()
             )
             .ReturnsAsync(responseMessage);

            var result = await _weatherService.GetWeatherForecastAsync(city);

            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetWeatherForecastAsync_ReturnsErrorResponse_WhenApiResponseIsUnsuccessful()
        {
            var city = "London";
            var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Bad Request")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(responseMessage);

            var result = await _weatherService.GetWeatherForecastAsync(city);

            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("Failed to fetch weather data. Status: BadRequest", result.Message);
            Assert.Null(result.Data);
        }

    }
}

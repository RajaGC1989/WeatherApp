using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using WeatherApp.API;
using WeatherApp.Domain.Models;
using Xunit;

namespace WeatherApp.IntegrationTests
{
    public class WeatherControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public WeatherControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetWeather_ShouldReturnSuccess()
        {
            var response = await _client.GetAsync("/api/weather");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var weatherData = await response.Content.ReadFromJsonAsync<List<Weather>>();
            weatherData.Should().NotBeNull();
        }
    }
}

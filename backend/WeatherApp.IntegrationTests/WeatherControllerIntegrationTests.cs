using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http;
using System.Threading.Tasks;

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
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }
    }
}

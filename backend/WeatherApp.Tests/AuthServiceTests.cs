using Microsoft.Extensions.Configuration;
using Moq;
using WeatherApp.Application.Services;
using WeatherApp.Domain.DTO;
using WeatherApp.Domain.Interfaces;
using WeatherApp.Domain.Models;
using Xunit;

namespace WeatherApp.UnitTests
{
    public class AuthServiceTests
    {
        private readonly AuthService _authService;
        private readonly Mock<IAuthRepository> _authRepositoryMock;
        private readonly Mock<IConfiguration> _configMock;

        public AuthServiceTests()
        {
            _authRepositoryMock = new Mock<IAuthRepository>();
            _configMock = new Mock<IConfiguration>();

            _authService = new AuthService(_authRepositoryMock.Object, _configMock.Object);
        }

        [Fact]
        public async Task RegisterUser_ShouldCreateUser_WhenValidInput()
        {
            // Arrange
            var newUser = new UserLoginDto { Username = "testuser", Password = "password123" };
            _authRepositoryMock.Setup(repo => repo.AddUserAsync(It.IsAny<User>())).ReturnsAsync(true);

            // Act
            var result = await _authService.RegisterUser(newUser);

            // Assert
            Assert.True(result);
            _authRepositoryMock.Verify(repo => repo.AddUserAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task LoginUser_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var user = new User { Username = "testuser", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123") };
            var loginDto = new UserLoginDto { Username = "testuser", Password = "password123" };
            _authRepositoryMock.Setup(repo => repo.GetUserByUsernameAsync("testuser")).ReturnsAsync(user);

            var secretKeySectionMock = new Mock<IConfigurationSection>();
            secretKeySectionMock.Setup(x => x.Value).Returns("4meFIcYrX6YQiY/ATfB0UgahONKinstSC3VXkGs0LFw=");

            _configMock.Setup(config => config.GetSection("SecretKey")).Returns(secretKeySectionMock.Object);


            // Act
            var token = await _authService.AuthenticateUser(loginDto);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
            _authRepositoryMock.Verify(repo => repo.GetUserByUsernameAsync("testuser"), Times.Once);
        }

        [Fact]
        public async Task LoginUser_ShouldReturnNull_WhenInvalidCredentials()
        {
            // Arrange
            var loginDto = new UserLoginDto { Username = "testuser", Password = "wrongpassword" };
            _authRepositoryMock.Setup(repo => repo.GetUserByUsernameAsync("testuser")).ReturnsAsync((User)null);

            // Act
            var token = await _authService.AuthenticateUser(loginDto);

            // Assert
            Assert.Null(token);
            _authRepositoryMock.Verify(repo => repo.GetUserByUsernameAsync("testuser"), Times.Once);
        }
    }
}

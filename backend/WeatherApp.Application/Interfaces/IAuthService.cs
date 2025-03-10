using WeatherApp.Domain.DTO;

namespace WeatherApp.Application.Services
{
    public interface IAuthService
    {
        Task<bool> RegisterUser(UserLoginDto newUser);
        Task<string?> AuthenticateUser(UserLoginDto login);
    }
}

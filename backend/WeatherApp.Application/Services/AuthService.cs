using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WeatherApp.Domain.DTO;
using WeatherApp.Domain.Interfaces;
using WeatherApp.Domain.Models;

namespace WeatherApp.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _config;

        public AuthService(IAuthRepository authRepository, IConfiguration config)
        {
            _authRepository = authRepository;
            _config = config;
        }

        public Task<string?> AuthenticateUser(UserLoginDto login)
        {
            var user = _authRepository.GetUserByUsernameAsync(login.Username).Result;
            if (user != null && BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
            {
                var token = GenerateJwtToken(user.Username);
                return Task.FromResult(token);
            }
            return Task.FromResult<string?>(null);
        }

        public async Task<bool> RegisterUser(UserLoginDto newUser)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newUser.Password);

            var user = new User
            {
                Username = newUser.Username,
                PasswordHash = hashedPassword
            };

            return await _authRepository.AddUserAsync(user);
        }

        private string GenerateJwtToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config.GetSection("SecretKey").Value);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

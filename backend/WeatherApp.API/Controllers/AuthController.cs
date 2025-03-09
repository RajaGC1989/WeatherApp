using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WeatherApp.API.DTO;
using WeatherApp.Domain.Models;
using WeatherApp.Infrastructure.Persistance;

namespace WeatherApp.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _appDbContext;

        public AuthController(IConfiguration config, AppDbContext appDbContext)
        {
            _config = config;
            _appDbContext = appDbContext;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserLoginDto newUser)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newUser.Password);

            var user = new User
            {
                Username = newUser.Username,
                PasswordHash = hashedPassword
            };

            _appDbContext.Users.Add(user);
            await _appDbContext.SaveChangesAsync();

            return Created($"/api/users/{user.Username}", user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto login)
        {
            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Username == login.Username);

            if (user != null || BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
            {
                var token = GenerateJwtToken(user.Username);
                return Ok(new { token });
            }

            return Unauthorized("Invalid credentials");
        }

        private string GenerateJwtToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config.GetValue<string>("SecretKey"));

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

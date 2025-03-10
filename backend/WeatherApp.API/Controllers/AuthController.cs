using Microsoft.AspNetCore.Mvc;
using WeatherApp.Application.Services;
using WeatherApp.Domain.DTO;
using WeatherApp.Infrastructure.Persistance;

namespace WeatherApp.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IConfiguration config, AppDbContext appDbContext, IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserLoginDto newUser)
        {
            if (newUser == null)
            {
                return BadRequest("Invalid user data");
            }

            var result = await _authService.RegisterUser(newUser);

            if (!result)
            {
                return BadRequest("User already exists");
            }

            return Created($"/api/users/{newUser.Username}", newUser);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto login)
        {
            if (login == null)
            {
                return BadRequest("Invalid login data");
            }

            var token = await _authService.AuthenticateUser(login);

            return token != null ? Ok(new { token }) : Unauthorized("Invalid credentials");
        }
    }
}

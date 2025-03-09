using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WeatherApp.API.DTO;
using WeatherApp.Application.CQRS.Commands;
using WeatherApp.Application.CQRS.Queries;
using WeatherApp.Application.Services;
using WeatherApp.Domain.Interfaces;
using WeatherApp.Domain.Models;
using WeatherApp.Infrastructure.Persistance;
using WeatherApp.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // Allow only React frontend
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{ options.UseSqlite("Data Source=weather.db"); });

builder.Services.AddScoped<IWeatherRepository, WeatherRepository>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetAllWeatherQuery).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateWeatherCommand).Assembly));

builder.Services.AddHttpClient();

builder.Services.Configure<WeatherApiSettings>(builder.Configuration.GetSection("WeatherApi"));
builder.Services.AddScoped<WeatherService>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var secreyKey = builder.Configuration.GetValue<string>("SecretKey");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secreyKey));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = key,
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/auth/register", async (AppDbContext db, UserLoginDto register) =>
{
    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(register.Password);

    var user = new User
    {
        Username = register.Username,
        PasswordHash = hashedPassword
    };

    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Created($"/api/users/{user.Username}", user);
});

app.MapPost("/auth/login", async (AppDbContext db, UserLoginDto login) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == login.Username);

    if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
    {
        return Results.Unauthorized();
    }

    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("SecretKey"));

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.Username) }),
        Expires = DateTime.UtcNow.AddHours(1),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    var tokenString = tokenHandler.WriteToken(token);

    return Results.Ok(new { Token = tokenString });
});

app.MapGet("/api/weather", async (IMediator mediator) =>
{
    var result = mediator.Send(new GetAllWeatherQuery());
    return Results.Ok(await result);
}).RequireAuthorization();

app.MapGet("api/weather/{city}", async (IMediator mediator, string city) =>
{
    var result = mediator.Send(new GetWeatherByCity(city));
    if (result.Result == null)
        return Results.NotFound("Please add the city");

    return Results.Ok(result.Result);
}).RequireAuthorization();

app.MapPost("api/weather", async (IMediator mediator, WeatherService weatherService, string city) =>
{
    var weather = await weatherService.GetWeatherForecastAsync(city);
    if (weather == null)
        return Results.NotFound();

    await mediator.Send(new CreateWeatherCommand(weather));
    return Results.Created($"/api/weather/{weather.CityName}", weather);
}).RequireAuthorization();

app.MapPut("api/weather/{city}", async (IMediator mediator, WeatherService weatherService, string city) =>
{
    var weather = await weatherService.GetWeatherForecastAsync(city);
    if (weather == null)
        return Results.NotFound();

    await mediator.Send(new UpdateWeatherCommand(weather));
    return Results.NoContent();
}).RequireAuthorization();

app.MapDelete("api/weather/{city}", async (IMediator mediator, string city) =>
{
    await mediator.Send(new DeleteWeatherCommand(city));
    return Results.NoContent();
}).RequireAuthorization();


app.MapGet("api/weather/fetch/{city}", async (IMediator mediator, WeatherService weatherService, string city) =>
{
    var weather = await weatherService.GetWeatherForecastAsync(city);
    if (weather == null)
        return Results.NotFound();

    await mediator.Send(new CreateWeatherCommand(weather));
    return Results.Ok(weather);
}).RequireAuthorization();

await app.RunAsync();
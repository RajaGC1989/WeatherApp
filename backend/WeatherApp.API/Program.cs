using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WeatherApp.Application.CQRS.Commands;
using WeatherApp.Application.CQRS.Queries;
using WeatherApp.Application.Interfaces;
using WeatherApp.Application.Services;
using WeatherApp.Domain.Interfaces;
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

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Weather API", Version = "v1" });

    // Enable JWT Authentication in Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{ options.UseSqlite("Data Source=weather.db"); });

builder.Services.AddScoped<IWeatherRepository, WeatherRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetAllWeatherQuery).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateWeatherCommand).Assembly));

builder.Services.AddHttpClient();

builder.Services.Configure<WeatherApiSettings>(builder.Configuration.GetSection("WeatherApi"));
builder.Services.AddScoped<IWeatherService, WeatherService>();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather API v1"));
}

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

await app.RunAsync();
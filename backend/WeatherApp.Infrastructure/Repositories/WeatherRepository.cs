﻿using Microsoft.EntityFrameworkCore;
using WeatherApp.Domain.Interfaces;
using WeatherApp.Domain.Models;
using WeatherApp.Infrastructure.Persistance;

namespace WeatherApp.Infrastructure.Repositories
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly AppDbContext _context;

        public WeatherRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Weather> AddWeatherAsync(Weather weather)
        {
            try
            {
                var weatherByCity = GetWeatherByCityAsync(weather.CityName);

                if (weatherByCity.Result != null)
                {
                    return await UpdateWeatherAsync(weather);
                }
                else
                {
                    _context.Weathers.Add(weather);
                    await _context.SaveChangesAsync();
                    return await GetWeatherByCityAsync(weather.CityName);
                }
            }
            catch (DbUpdateException dbEx)
            {
                // Log database update exceptions
                // Log(dbEx);
                //throw new Exception("An error occurred while updating the database.", dbEx);
                return null;
            }
            catch (Exception ex)
            {
                // Log general exceptions
                // Log(ex);
                //throw new Exception("An error occurred while adding weather data.", ex);
                return null;
            }
        }

        public async Task<bool> DeleteWeatherAsync(string city)
        {
            var weather = await GetWeatherByCityAsync(city);
            if (weather != null)
            {
                _context.Weathers.Remove(weather);
                await _context.SaveChangesAsync();

                return true;
            }
            return false;
        }

        public async Task<List<Weather>> GetAllWeatherAsync() => await _context.Weathers.ToListAsync();

        public async Task<Weather> GetWeatherByCityAsync(string city)
        {
            return await _context.Weathers.FirstOrDefaultAsync(w => w.CityName == city);
        }

        public async Task<Weather> UpdateWeatherAsync(Weather weather)
        {
            var weatherByCity = GetWeatherByCityAsync(weather.CityName);

            if(weatherByCity != null)
            {
                _context.Weathers.Update(weather);
                await _context.SaveChangesAsync();
            }
            
            return await GetWeatherByCityAsync(weather.CityName);
        }

    }
}

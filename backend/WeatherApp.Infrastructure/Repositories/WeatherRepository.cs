using Microsoft.EntityFrameworkCore;
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
                var weatherByCity = await GetWeatherByCityAsync(weather.CityName);

                if (weatherByCity != null)
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
                // Todo: Log database update exceptions
                return null;
            }
            catch (Exception ex)
            {
                // Log general exceptions
                // Log(ex);
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
            var existingWeather = await GetWeatherByCityAsync(weather.CityName);

            if (existingWeather != null)
            {
                var trackedEntity = _context.ChangeTracker.Entries<Weather>()
                                            .FirstOrDefault(e => e.Entity.Id == existingWeather.Id);
                if (trackedEntity != null)
                {
                    _context.Entry(trackedEntity.Entity).State = EntityState.Detached;
                }

                weather.Id = existingWeather.Id;
                _context.Weathers.Attach(weather);
                _context.Entry(weather).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                return weather;
            }

            return null;
        }

    }
}

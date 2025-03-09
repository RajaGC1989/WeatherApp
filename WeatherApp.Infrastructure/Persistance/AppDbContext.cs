using Microsoft.EntityFrameworkCore;
using WeatherApp.Domain.Models;

namespace WeatherApp.Infrastructure.Persistance
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Weather> Weathers => Set<Weather>();
        public DbSet<User> Users => Set<User>();
    }
}

using WeatherApp.Domain.Interfaces;
using WeatherApp.Domain.Models;
using WeatherApp.Infrastructure.Persistance;

namespace WeatherApp.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _appdbContext;

        public AuthRepository(AppDbContext appDbContext)
        {
            _appdbContext = appDbContext;
        }

        public async Task<bool> AddUserAsync(User user)
        {
            try
            {
                await _appdbContext.Users.AddAsync(user);
                await _appdbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Task<User?> GetUserByUsernameAsync(string username)
        {
            return Task.FromResult(_appdbContext.Users.FirstOrDefault(u => u.Username == username));
        }
    }
}

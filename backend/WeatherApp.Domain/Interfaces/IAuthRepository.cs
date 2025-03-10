using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Domain.Models;

namespace WeatherApp.Domain.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task<bool> AddUserAsync(User user);
    }
}

using Jwt18.Entities;
using Jwt18.Models;

namespace Jwt18.Services
{
    public interface IAuthService
    {
          Task<User?> RegisterAsync(UserDto request);
          Task<string?> LoginAsync(UserDto request);
    }
}

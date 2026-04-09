using Models;

namespace SkischoolHub.Services;

public interface IApiService
{
    Task<User?> LoginAsync(string email, string password);
    Task<bool> RegisterUserAsync(User user);
    Task<User?> GetUserByIdAsync(int id);
}
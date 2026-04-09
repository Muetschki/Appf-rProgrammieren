using System.Net.Http.Json;
using Models;

namespace SkischoolHub.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        var request = new
        {
            Email = email,
            Password = password
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/users/login", request);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<User>();
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> RegisterUserAsync(User user)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/users", user);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
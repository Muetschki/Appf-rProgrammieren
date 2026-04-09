using Models;
using System.Net.Http.Json;

namespace SkischoolHub.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;

    public ApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7001")
        };
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        try
        {
            var loginRequest = new { Email = email, Password = password };
            var response = await _httpClient.PostAsJsonAsync("api/users/login", loginRequest);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<User>();
            }

            System.Diagnostics.Debug.WriteLine($"Login fehlgeschlagen: {response.StatusCode}");
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> RegisterUserAsync(User user)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"Sende Registrierung für: {user.Email}");
            var response = await _httpClient.PostAsJsonAsync("api/users", user);
            System.Diagnostics.Debug.WriteLine($"Response Status: {response.StatusCode}");
            
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Fehler-Inhalt: {content}");
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Registration error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            return false;
        }
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<User>($"api/users/{id}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Get user error: {ex.Message}");
            return null;
        }
    }
}
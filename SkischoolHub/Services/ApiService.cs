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
        var request = new { Email = email, Password = password };
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/users/login", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<User>();
        }
        catch { return null; }
    }

    public async Task<bool> RegisterUserAsync(User user)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/users", user);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<List<SkiCourse>> GetSkiCoursesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<SkiCourse>>("api/skicourses") ?? [];
        }
        catch { return []; }
    }

    public async Task<List<CourseBooking>> GetUserBookingsAsync(int userId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<CourseBooking>>($"api/skicourses/user/{userId}/bookings") ?? [];
        }
        catch { return []; }
    }

    public async Task<bool> BookCourseAsync(int userId, int courseId)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"api/skicourses/{courseId}/bookings", new { UserId = userId });
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> CancelCourseBookingAsync(int userId, int courseId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/skicourses/{courseId}/bookings/{userId}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    // ─── Privatstunden ───────────────────────────────────────────────────────

    public async Task<List<SkiTeacher>> GetSkiTeachersAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<SkiTeacher>>("api/privatelessons/teachers") ?? [];
        }
        catch { return []; }
    }

    public async Task<PrivateLesson?> BookPrivateLessonAsync(int userId, int teacherId, DateTime date, string timeSlot)
    {
        try
        {
            var request = new
            {
                UserId = userId,
                SkiTeacherId = teacherId,
                LessonDate = date,
                TimeSlot = timeSlot
            };
            var response = await _httpClient.PostAsJsonAsync("api/privatelessons", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<PrivateLesson>();
        }
        catch { return null; }
    }

    public async Task<List<PrivateLesson>> GetPrivateLessonsAsync(int userId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<PrivateLesson>>($"api/privatelessons/user/{userId}") ?? [];
        }
        catch { return []; }
    }

    public async Task<bool> CancelPrivateLessonAsync(int lessonId, int userId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/privatelessons/{lessonId}/user/{userId}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }
}
using Models;

namespace SkischoolHub.Services;

public interface IApiService
{
    Task<User?> LoginAsync(string email, string password);
    Task<bool> RegisterUserAsync(User user);
    Task<List<SkiCourse>> GetSkiCoursesAsync();
    Task<List<CourseBooking>> GetUserBookingsAsync(int userId);
    Task<bool> BookCourseAsync(int userId, int courseId);
    Task<bool> CancelCourseBookingAsync(int userId, int courseId);
}
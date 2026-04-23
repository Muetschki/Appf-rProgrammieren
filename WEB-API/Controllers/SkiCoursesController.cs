using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using ORM;

namespace WEB_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SkiCoursesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SkiCoursesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetCourses()
    {
        var courses = await _context.SkiCourses
            .OrderBy(c => c.Date)
            .ToListAsync();

        return Ok(courses);
    }

    [HttpGet("user/{userId:int}/bookings")]
    public async Task<IActionResult> GetBookingsForUser(int userId)
    {
        var bookings = await _context.CourseBookings
            .Where(b => b.UserId == userId)
            .ToListAsync();

        return Ok(bookings);
    }

    [HttpPost("{courseId:int}/bookings")]
    public async Task<IActionResult> BookCourse(int courseId, [FromBody] BookCourseRequest request)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);
        if (!userExists)
        {
            return NotFound("Benutzer wurde nicht gefunden.");
        }

        var course = await _context.SkiCourses.FirstOrDefaultAsync(c => c.Id == courseId);
        if (course is null)
        {
            return NotFound("Kurs wurde nicht gefunden.");
        }

        var alreadyBooked = await _context.CourseBookings
            .AnyAsync(b => b.UserId == request.UserId && b.SkiCourseId == courseId);

        if (alreadyBooked)
        {
            return Conflict("Dieser Kurs wurde bereits gebucht.");
        }

        var currentBookings = await _context.CourseBookings.CountAsync(b => b.SkiCourseId == courseId);
        if (currentBookings >= course.MaxParticipants)
        {
            return Conflict("Dieser Kurs ist bereits ausgebucht.");
        }

        var booking = new CourseBooking
        {
            UserId = request.UserId,
            SkiCourseId = courseId,
            BookedAt = DateTime.UtcNow
        };

        _context.CourseBookings.Add(booking);
        await _context.SaveChangesAsync();

        return Ok(booking);
    }

    [HttpDelete("{courseId:int}/bookings/{userId:int}")]
    public async Task<IActionResult> CancelBooking(int courseId, int userId)
    {
        var booking = await _context.CourseBookings
            .FirstOrDefaultAsync(b => b.SkiCourseId == courseId && b.UserId == userId);

        if (booking is null)
        {
            return NotFound("Buchung wurde nicht gefunden.");
        }

        _context.CourseBookings.Remove(booking);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public class BookCourseRequest
{
    public int UserId { get; set; }
}

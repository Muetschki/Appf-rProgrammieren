using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using ORM;

namespace WEB_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PrivateLessonsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PrivateLessonsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET api/privatelessons/teachers
    [HttpGet("teachers")]
    public async Task<IActionResult> GetTeachers()
    {
        var teachers = await _context.SkiTeachers
            .Where(t => t.IsAvailable)
            .OrderBy(t => t.LastName)
            .ToListAsync();
        return Ok(teachers);
    }

    // GET api/privatelessons/user/{userId}
    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetLessonsForUser(int userId)
    {
        var lessons = await _context.PrivateLessons
            .Include(l => l.SkiTeacher)
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.LessonDate)
            .ToListAsync();
        return Ok(lessons);
    }

    // GET api/privatelessons/teachers/{teacherId}/availability?date=2026-02-01
    [HttpGet("teachers/{teacherId:int}/availability")]
    public async Task<IActionResult> GetTeacherAvailability(int teacherId, [FromQuery] DateTime date)
    {
        var bookedSlots = await _context.PrivateLessons
            .Where(l => l.SkiTeacherId == teacherId && l.LessonDate.Date == date.Date)
            .Select(l => l.TimeSlot)
            .ToListAsync();

        return Ok(new { bookedSlots });
    }

    // POST api/privatelessons
    [HttpPost]
    public async Task<IActionResult> BookPrivateLesson([FromBody] BookPrivateLessonRequest request)
    {
        if (request == null)
            return BadRequest("Ungültige Anfrage");

        var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);
        if (!userExists)
            return NotFound("Benutzer wurde nicht gefunden.");

        var teacher = await _context.SkiTeachers.FirstOrDefaultAsync(t => t.Id == request.SkiTeacherId);
        if (teacher is null)
            return NotFound("Lehrer wurde nicht gefunden.");

        if (!teacher.IsAvailable)
            return Conflict("Dieser Lehrer ist derzeit nicht verfügbar.");

        // Doppelbuchung verhindern: gleicher Lehrer, gleiches Datum, gleiche Uhrzeit
        var conflict = await _context.PrivateLessons.AnyAsync(l =>
            l.SkiTeacherId == request.SkiTeacherId &&
            l.LessonDate.Date == request.LessonDate.Date &&
            l.TimeSlot == request.TimeSlot);

        if (conflict)
            return Conflict($"Dieser Lehrer ist am {request.LessonDate:dd.MM.yyyy} um {request.TimeSlot} Uhr bereits gebucht.");

        var lesson = new PrivateLesson
        {
            UserId = request.UserId,
            SkiTeacherId = request.SkiTeacherId,
            LessonDate = request.LessonDate.Date,
            TimeSlot = request.TimeSlot,
            Price = 150.00m,
            BookedAt = DateTime.UtcNow
        };

        _context.PrivateLessons.Add(lesson);
        await _context.SaveChangesAsync();

        await _context.Entry(lesson).Reference(l => l.SkiTeacher).LoadAsync();
        return Ok(lesson);
    }

    // DELETE api/privatelessons/{lessonId}/user/{userId}
    [HttpDelete("{lessonId:int}/user/{userId:int}")]
    public async Task<IActionResult> CancelLesson(int lessonId, int userId)
    {
        var lesson = await _context.PrivateLessons
            .FirstOrDefaultAsync(l => l.Id == lessonId && l.UserId == userId);

        if (lesson is null)
            return NotFound("Privatstunde wurde nicht gefunden.");

        _context.PrivateLessons.Remove(lesson);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

public class BookPrivateLessonRequest
{
    public int UserId { get; set; }
    public int SkiTeacherId { get; set; }
    public DateTime LessonDate { get; set; }
    public string TimeSlot { get; set; } = string.Empty;
}
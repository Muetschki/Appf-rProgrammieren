using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using ORM;

namespace WEB_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        if (user == null || string.IsNullOrWhiteSpace(user.Email))
            return BadRequest("Ungültige Daten");

        if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            return Conflict("Email bereits registriert");

        // Passwort hashen mit BCrypt
        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var result = new { user.Id, user.FirstName, user.LastName, user.Email };
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
            return Unauthorized(new { message = "Ungültige Anmeldedaten" });

        // BCrypt Passwort-Vergleich
        bool passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

        if (!passwordValid)
            return Unauthorized(new { message = "Ungültige Anmeldedaten" });

        var result = new { user.Id, user.FirstName, user.LastName, user.Email };
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        var result = new { user.Id, user.FirstName, user.LastName, user.Email };
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _context.Users
            .Select(u => new { u.Id, u.FirstName, u.LastName, u.Email })
            .ToListAsync();
        return Ok(users);
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
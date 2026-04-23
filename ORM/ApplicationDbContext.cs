using Microsoft.EntityFrameworkCore;
using Models;

namespace ORM;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<SkiCourse> SkiCourses { get; set; } = null!;
    public DbSet<CourseBooking> CourseBookings { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
        });

        modelBuilder.Entity<SkiCourse>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Title).IsRequired().HasMaxLength(120);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(400);
            entity.Property(e => e.Price).HasColumnType("decimal(10,2)");

            entity.HasData(
                new SkiCourse
                {
                    Id = 1,
                    Title = "Anfänger Kurs",
                    Description = "Die perfekte Einführung ins Skifahren.",
                    Date = new DateTime(2026, 1, 10, 9, 0, 0),
                    Price = 89.00m,
                    MaxParticipants = 12
                },
                new SkiCourse
                {
                    Id = 2,
                    Title = "Fortgeschrittene Technik",
                    Description = "Verbessern Sie Ihre Technik mit Profi-Tipps.",
                    Date = new DateTime(2026, 1, 17, 9, 0, 0),
                    Price = 119.00m,
                    MaxParticipants = 10
                },
                new SkiCourse
                {
                    Id = 3,
                    Title = "Carving Intensiv",
                    Description = "Intensivtraining für saubere Carving-Schwünge.",
                    Date = new DateTime(2026, 1, 24, 9, 0, 0),
                    Price = 129.00m,
                    MaxParticipants = 8
                });
        });

        modelBuilder.Entity<CourseBooking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.BookedAt).IsRequired();
            entity.HasIndex(e => new { e.UserId, e.SkiCourseId }).IsUnique();
        });
    }
}
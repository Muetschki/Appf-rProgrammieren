using Microsoft.EntityFrameworkCore;
using ORM;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var serverVersion = new MySqlServerVersion(new Version(8, 0, 21));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, serverVersion));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    await db.Database.ExecuteSqlRawAsync(@"
        CREATE TABLE IF NOT EXISTS `SkiCourses` (
            `Id` INT NOT NULL AUTO_INCREMENT,
            `Title` VARCHAR(120) NOT NULL,
            `Description` VARCHAR(400) NOT NULL,
            `Date` DATETIME(6) NOT NULL,
            `Price` DECIMAL(10,2) NOT NULL,
            `MaxParticipants` INT NOT NULL,
            PRIMARY KEY (`Id`)
        );");

    await db.Database.ExecuteSqlRawAsync(@"
        CREATE TABLE IF NOT EXISTS `CourseBookings` (
            `Id` INT NOT NULL AUTO_INCREMENT,
            `UserId` INT NOT NULL,
            `SkiCourseId` INT NOT NULL,
            `BookedAt` DATETIME(6) NOT NULL,
            PRIMARY KEY (`Id`),
            CONSTRAINT `FK_CourseBookings_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE,
            CONSTRAINT `FK_CourseBookings_SkiCourses_SkiCourseId` FOREIGN KEY (`SkiCourseId`) REFERENCES `SkiCourses` (`Id`) ON DELETE CASCADE,
            UNIQUE KEY `IX_CourseBookings_UserId_SkiCourseId` (`UserId`,`SkiCourseId`)
        );");

    // skiteachers existiert bereits – wird NICHT neu erstellt!
    // Nur PrivateLessons neu anlegen:
    await db.Database.ExecuteSqlRawAsync(@"
        CREATE TABLE IF NOT EXISTS `PrivateLessons` (
            `Id` INT NOT NULL AUTO_INCREMENT,
            `UserId` INT NOT NULL,
            `SkiTeacherId` INT NOT NULL,
            `LessonDate` DATETIME(6) NOT NULL,
            `TimeSlot` VARCHAR(10) NOT NULL,
            `Price` DECIMAL(10,2) NOT NULL,
            `BookedAt` DATETIME(6) NOT NULL,
            PRIMARY KEY (`Id`),
            CONSTRAINT `FK_PrivateLessons_Users` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE,
            CONSTRAINT `FK_PrivateLessons_SkiTeachers` FOREIGN KEY (`SkiTeacherId`) REFERENCES `skiteachers` (`Id`) ON DELETE RESTRICT,
            UNIQUE KEY `UX_PrivateLessons_Teacher_Date_Time` (`SkiTeacherId`, `LessonDate`, `TimeSlot`)
        );");

    if (!await db.SkiCourses.AnyAsync())
    {
        db.SkiCourses.AddRange(
            new Models.SkiCourse { Title = "Anfänger Kurs", Description = "Die perfekte Einführung ins Skifahren.", Date = new DateTime(2026, 1, 10, 9, 0, 0), Price = 89.00m, MaxParticipants = 12 },
            new Models.SkiCourse { Title = "Fortgeschrittene Technik", Description = "Verbessern Sie Ihre Technik mit Profi-Tipps.", Date = new DateTime(2026, 1, 17, 9, 0, 0), Price = 119.00m, MaxParticipants = 10 },
            new Models.SkiCourse { Title = "Carving Intensiv", Description = "Intensivtraining für saubere Carving-Schwünge.", Date = new DateTime(2026, 1, 24, 9, 0, 0), Price = 129.00m, MaxParticipants = 8 }
        );
        await db.SaveChangesAsync();
    }
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
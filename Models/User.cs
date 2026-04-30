using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class SkiCourse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public int MaxParticipants { get; set; }
    }

    public class CourseBooking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SkiCourseId { get; set; }
        public DateTime BookedAt { get; set; }
    }

    public class SkiTeacher
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;

        // Nur im Code, nicht in der DB
        public string FullName => $"{FirstName} {LastName}";
    }

    public class PrivateLesson
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SkiTeacherId { get; set; }
        public DateTime LessonDate { get; set; }
        public string TimeSlot { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime BookedAt { get; set; }

        public SkiTeacher? SkiTeacher { get; set; }
    }
}
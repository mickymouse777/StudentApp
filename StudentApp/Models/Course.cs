using System.ComponentModel.DataAnnotations;

namespace StudentApp.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }
        public string? CourseName { get; set; }
        public string? Description { get; set; }

        // Navigation property for enrollments
        public List<Enrollment>? Enrollments { get; set; }
    }
}

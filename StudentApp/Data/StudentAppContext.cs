using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentApp.Models;

namespace StudentApp.Data
{
    public class StudentAppContext : DbContext
    {
        public StudentAppContext (DbContextOptions<StudentAppContext> options)
            : base(options)
        {
        }

        public DbSet<StudentApp.Models.Student> Student { get; set; } = default!;
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<EnrollmentHistory> EnrollmentHistories { get; set; }
    }
}

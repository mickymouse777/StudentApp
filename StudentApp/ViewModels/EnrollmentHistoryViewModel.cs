using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using StudentApp.Models;

namespace StudentApp.ViewModels
{
    public class EnrollmentHistoryViewModel
    {
        public List<Enrollment>? Enrollments { get; set; }

        public string? FilterCourseName { get; set; }
        public string? FilterStudentName { get; set; }
        public DateTime? FilterStartDate { get; set; }
        public DateTime? FilterEndDate { get; set; }
    }
}

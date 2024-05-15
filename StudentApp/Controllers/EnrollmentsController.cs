using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentApp.Data;
using StudentApp.Models;
using StudentApp.ViewModels;

namespace StudentApp.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly StudentAppContext _context;

        public EnrollmentsController(StudentAppContext context)
        {
            _context = context;
        }

        // GET: Enrollments
        public async Task<IActionResult> Index()
        {
            var studentAppContext = _context.Enrollments.Include(e => e.Course).Include(e => e.Student);
            return View(await studentAppContext.ToListAsync());
        }

        // GET: Enrollments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.EnrollmentId == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // GET: Enrollments/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseName");
            ViewData["StudentId"] = new SelectList(_context.Student, "StudentId", "StudentName");
            return View();
        }

        // POST: Enrollments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EnrollmentId,CourseId,StudentId")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                // Set the enrollment date when creating a new enrollment
                enrollment.EnrollmentDate = DateTime.Now;
                enrollment.ModifiedDate = DateTime.Now;  // Initially, ModifiedDate is the same as EnrollmentDate

                _context.Add(enrollment);
                await _context.SaveChangesAsync();

                // Add to history
                var history = new EnrollmentHistory
                {
                    EnrollmentId = enrollment.EnrollmentId,
                    ChangeDate = DateTime.Now,
                };
                _context.EnrollmentHistories.Add(history);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseName", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "StudentId", "Name", enrollment.StudentId);
            return View(enrollment);
        }

        // GET: Enrollments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseId", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "StudentId", "StudentId", enrollment.StudentId);
            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EnrollmentId,CourseId,StudentId")] Enrollment enrollment)
        {
            if (id != enrollment.EnrollmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    enrollment.ModifiedDate = DateTime.Now;
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.EnrollmentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseId", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "StudentId", "StudentId", enrollment.StudentId);
            return View(enrollment);
        }

        // GET: Enrollments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.EnrollmentId == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //public async Task<IActionResult> History(int? studentId)
        //{
        //    var students = await _context.Student.OrderBy(s => s.StudentName).ToListAsync();
        //    var studentsSelectList = new SelectList(students, "StudentId", "Name");

        //    List<Enrollment> enrollments; // Declare variable
        //    if (studentId.HasValue && students.Any())
        //    {
        //        enrollments = await _context.Enrollments.Include(e => e.Course).Include(e => e.Student)
        //                            .Where(e => e.StudentId == studentId)
        //                            .OrderByDescending(e => e.ModifiedDate).ToListAsync();
        //    }
        //    else
        //    {
        //        enrollments = new List<Enrollment>(); // Always initialize
        //    }

        //    var viewModel = new EnrollmentHistoryViewModel
        //    {
        //        SelectedStudentId = studentId,
        //        Student = studentsSelectList,
        //        Enrollments = enrollments // Pass the initialized list
        //    };

        //    return View(viewModel);
        //}


        public async Task<IActionResult> EnrollmentHistory(EnrollmentHistoryViewModel model)
        {
            var query = _context.Enrollments.Include(e => e.Course).Include(e => e.Student).AsQueryable();

            if (!string.IsNullOrEmpty(model.FilterCourseName))
            {
                query = query.Where(e => e.Course.CourseName.Contains(model.FilterCourseName));
            }

            if (!string.IsNullOrEmpty(model.FilterStudentName))
            {
                query = query.Where(e => e.Student.StudentName.Contains(model.FilterStudentName));
            }

            if (model.FilterStartDate.HasValue)
            {
                query = query.Where(e => e.EnrollmentDate >= model.FilterStartDate.Value);
            }

            if (model.FilterEndDate.HasValue)
            {
                query = query.Where(e => e.EnrollmentDate <= model.FilterEndDate.Value);
            }

            model.Enrollments = await query.OrderByDescending(e => e.ModifiedDate).ToListAsync();

            return View(model);
        }



        private bool EnrollmentExists(int id)
        {
            return _context.Enrollments.Any(e => e.EnrollmentId == id);
        }
    }
}

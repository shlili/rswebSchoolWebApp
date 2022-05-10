#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCSchoolApp.Data;
using MVCSchoolApp.Models;
using MVCSchoolApp.ViewModels;

namespace MVCSchoolApp.Controllers
{
    public class StudentsController : Controller
    {
        private readonly MVCSchoolAppContext _context;

        public StudentsController(MVCSchoolAppContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index(string StudentFName, string StudentLName, long StudentIndex)
        {
            IQueryable<Student> students = _context.Student.AsQueryable();
            IQueryable<long> IDQuery = _context.Student.OrderBy(m => m.StudentId).Select(m => m.StudentId).Distinct();

            if (!string.IsNullOrEmpty(StudentFName) && (!string.IsNullOrEmpty(StudentLName)))
            {
                students = students.Where(c => c.FirstName.Contains(StudentFName)).Where(c => c.LastName.Contains(StudentLName));
            }

            else if (!string.IsNullOrEmpty(StudentFName) && (string.IsNullOrEmpty(StudentLName)))
            {
                students = students.Where(c => c.FirstName.Contains(StudentFName));
            }



            else if (string.IsNullOrEmpty(StudentFName) && (!string.IsNullOrEmpty(StudentLName)))
            {
                students = students.Where(c => c.LastName.Contains(StudentLName));
            }

            if (StudentIndex!=0)
            {
                students = students.Where(c => c.StudentId == StudentIndex);
            }

            students = students
                               .Include(c => c.Courses).ThenInclude(c => c.Course);
                               

            var StudentFilterVM = new StudentFilter
            {
                filteredS = new SelectList(await IDQuery.ToListAsync()),
                Students = await students.ToListAsync()
            };
            return View(StudentFilterVM);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentId,FirstName,LastName,EnrollmentDate,AcquiredCredits,CurrentSemester,EducationLevel")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("StudentId,FirstName,LastName,EnrollmentDate,AcquiredCredits,CurrentSemester,EducationLevel")] Student student)
        {
            if (id != student.StudentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.StudentId))
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
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var student = await _context.Student.FindAsync(id);
            _context.Student.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(long id)
        {
            return _context.Student.Any(e => e.StudentId == id);
        }

        public async Task<IActionResult> EnrolledStudents(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .FirstOrDefaultAsync(m => m.CourseId == id);

            IQueryable<Student> studentQuery = _context.Enrollment.Where(x => x.CourseId == id).Select(x => x.Student);
            await _context.SaveChangesAsync();
            if (course == null)
            {
                return NotFound();
            }

            ViewBag.Message = course.Title;
            var studentVM = new StudentViewModel2
            {
                Students = await studentQuery.ToListAsync(),
            };

            return View(studentVM);
        }
        /*private string UploadedFile(StudentProfilePicture viewmodel)
        {
            string uniqueFileName = null;

            if (viewmodel.ProfilePictureFile != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Pictures");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(viewmodel.ProfilePictureFile.FileName);
                string fileNameWithPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewmodel.ProfilePictureFile.CopyTo(stream);
                }
            }
            return uniqueFileName;
        }*/
    }
}

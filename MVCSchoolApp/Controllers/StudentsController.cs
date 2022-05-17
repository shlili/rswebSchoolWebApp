#nullable disable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using MVCSchoolApp.Data;
using MVCSchoolApp.Models;
using MVCSchoolApp.ViewModels;
using System.IO;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize]
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
            StudentImageVM viewmodel = new StudentImageVM
            {
                student = student,
                ProfileImageName = student.ProfilePicture
            };

            return View(viewmodel);
        }

        // GET: Students/Create
        [Authorize(Roles = "Admin")]

        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

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
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = _context.Student.Where(x => x.StudentId == id).Include(x => x.Courses).First();
            if (student == null)
            {
                return NotFound();
            }

            var courses = _context.Course.AsEnumerable();
            courses = courses.OrderBy(s => s.Title);

            StudentEdit viewmodel = new StudentEdit
            {
                student = student,
                coursesEnrolledList = new MultiSelectList(courses, "CourseId", "Title"),
                selectedCourses = student.Courses.Select(x => x.CourseId)
            };
            ViewData["Courses"] = new SelectList(_context.Set<Course>(), "CourseId", "Title");
            return View(viewmodel);
            
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(long id, StudentEdit viewmodel)
        {
            if (id != viewmodel.student.StudentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewmodel.student);
                    await _context.SaveChangesAsync();

                    var student = _context.Student.Where(x => x.StudentId == id).First();

                    IEnumerable<int> selectedCourses = viewmodel.selectedCourses;
                    if (selectedCourses != null)
                    {
                        IQueryable<Enrollment> toBeRemoved = _context.Enrollment.Where(s => !selectedCourses.Contains(s.CourseId) && s.StudentId == id);
                        _context.Enrollment.RemoveRange(toBeRemoved);

                        IEnumerable<int> existEnrollments = _context.Enrollment.Where(s => selectedCourses.Contains(s.CourseId) && s.StudentId == id).Select(s => s.CourseId);
                        IEnumerable<int> newEnrollments = selectedCourses.Where(s => !existEnrollments.Contains(s));

                        foreach (int courseId in newEnrollments)
                            _context.Enrollment.Add(new Enrollment { StudentId = id, CourseId = courseId, Semester = viewmodel.semester, Year = viewmodel.year });

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        IQueryable<Enrollment> toBeRemoved = _context.Enrollment.Where(s => s.StudentId == id);
                        _context.Enrollment.RemoveRange(toBeRemoved);
                        await _context.SaveChangesAsync();
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(viewmodel.student.StudentId))
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
            return View(viewmodel);
        }

        // GET: Students/Delete/5
        [Authorize(Roles = "Admin")]

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
        [Authorize(Roles = "Admin")]

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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditPicture(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = _context.Student.Where(x => x.StudentId == id).Include(x => x.Courses).First();
            if (student == null)
            {
                return NotFound();
            }

            var courses = _context.Course.AsEnumerable();
            courses = courses.OrderBy(s => s.Title);

            StudentImageVM viewmodel = new StudentImageVM
            {
                student = student,
                ProfileImageName = student.ProfilePicture
            };

            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditPicture(long id, StudentImageVM viewmodel)
        {
            if (id != viewmodel.student.StudentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (viewmodel.ProfileImage != null)
                    {
                        string uniqueFileName = UploadedFile(viewmodel);
                        viewmodel.student.ProfilePicture = uniqueFileName;
                    }
                    else
                    {
                        viewmodel.student.ProfilePicture = viewmodel.ProfileImageName;
                    }

                    _context.Update(viewmodel.student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(viewmodel.student.StudentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { id = viewmodel.student.StudentId });
            }
            return View(viewmodel);
        }

        private string UploadedFile(StudentImageVM model)
        {
            string uniqueFileName = null;

            if (model.ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ProfileImage.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfileImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}

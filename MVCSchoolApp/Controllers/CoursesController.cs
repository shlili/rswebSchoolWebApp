#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCSchoolApp.Data;
using MVCSchoolApp.Models;
using MVCSchoolApp.ViewModels;

namespace MVCSchoolApp.Controllers
{
    public class CoursesController : Controller
    {
        private readonly MVCSchoolAppContext _context;

        public CoursesController(MVCSchoolAppContext context)
        {
            _context = context;
        }

        // GET: Courses
        
        public async Task<IActionResult> Index(string CourseProg, string CTitle, int CourseSem)
        {
            IQueryable<Course> courses = _context.Course.AsQueryable();
            IQueryable<int?> semQuery = _context.Course.OrderBy(c => c.Semester).Select(c => c.Semester).Distinct();
            if (!string.IsNullOrEmpty(CourseProg) && (!string.IsNullOrEmpty(CTitle)))
            {
                courses = courses.Where(c => c.Programme.Contains(CourseProg)).Where(c => c.Title.Contains(CTitle));
            }

            else if (!string.IsNullOrEmpty(CourseProg) && (string.IsNullOrEmpty(CTitle)))
            {
                courses = courses.Where(c => c.Programme.Contains(CourseProg));
            }

            else if (string.IsNullOrEmpty(CourseProg) && (!string.IsNullOrEmpty(CTitle)))
            {
                courses = courses.Where(c => c.Title.Contains(CTitle));
            }

            if (CourseSem.Equals(1) || CourseSem.Equals(2) || CourseSem.Equals(3) || CourseSem.Equals(4) || CourseSem.Equals(5) || CourseSem.Equals(6) || CourseSem.Equals(7) || CourseSem.Equals(8))
            {
                courses = courses.Where(x => x.Semester == CourseSem);
            }

            courses = courses.Include(c => c.FirstTeacher)
                .Include(c => c.SecondTeacher)
                .Include(c => c.Students).ThenInclude(c => c.Student);

            var courseVM = new CourseFilter
            {
                filtered = new SelectList(await semQuery.ToListAsync()),
                Courses = await courses.ToListAsync(),
            };

            return View(courseVM);
        }

        // GET: Courses/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .Include(c => c.FirstTeacher)
                .Include(c => c.SecondTeacher)
                 .Include(c => c.Students).ThenInclude(c => c.Student)
                .FirstOrDefaultAsync(m => m.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["FirstTeacherId"] = new SelectList(_context.Teacher, "TeacherId", "FullName");
            ViewData["SecondTeacherId"] = new SelectList(_context.Teacher, "TeacherId", "FullName");
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("CourseId,Title,Credits,Semester,Programme,EducationLevel,FirstTeacherId,SecondTeacherId")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FirstTeacherId"] = new SelectList(_context.Teacher, "TeacherId", "FullName", course.FirstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(_context.Teacher, "TeacherId", "FullName", course.SecondTeacherId);
            return View(course);
        }

        // GET: Courses/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = _context.Course.Where(c => c.CourseId == id).Include(c => c.Students).First();
            if (course == null)
            {
                return NotFound();
            }

            var students = _context.Student.AsEnumerable();
            students = students.OrderBy(s => s.FullName);

            CourseStudents viewmodel = new CourseStudents
            {
                Course = course,
                StudentsList = new MultiSelectList(students, "StudentId", "FullName"),
                SelStudents = course.Students.Select(sa => sa.StudentId),
            };

            ViewData["FirstTeacherId"] = new SelectList(_context.Teacher, "TeacherId", "FullName", course.FirstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(_context.Teacher, "TeacherId", "FullName", course.SecondTeacherId);
            return View(viewmodel);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int id, CourseStudents viewmodel)
        {
            if (id != viewmodel.Course.CourseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewmodel.Course);
                    await _context.SaveChangesAsync();

                    IEnumerable<long> listStudents = viewmodel.SelStudents;
                    IQueryable<Enrollment> toBeRemoved = _context.Enrollment.Where(c => !listStudents.Contains(c.StudentId) && c.CourseId == id);
                    _context.Enrollment.RemoveRange(toBeRemoved);

                    IEnumerable<long> existStudents = _context.Enrollment.Where(c => listStudents.Contains(c.StudentId) && c.CourseId == id).Select(c => c.StudentId);
                    IEnumerable<long> newStudents = listStudents.Where(c => !existStudents.Contains(c));
                    foreach (long StudentId in newStudents)
                        _context.Enrollment.Add(new Enrollment { StudentId = StudentId, CourseId = id });

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(viewmodel.Course.CourseId))
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
            ViewData["FirstTeacherId"] = new SelectList(_context.Teacher, "TeacherId", "FullName", viewmodel.Course.FirstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(_context.Teacher, "TeacherId", "FullName", viewmodel.Course.SecondTeacherId);
            return View(viewmodel);
        }

        // GET: Courses/Delete/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .Include(c => c.FirstTeacher)
                .Include(c => c.SecondTeacher)
                .FirstOrDefaultAsync(m => m.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Course.FindAsync(id);
            _context.Course.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.CourseId == id);
        }
        [Authorize(Roles = "Admin,Teacher")]

        public async Task<IActionResult> TeachingCourse(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher
                .FirstOrDefaultAsync(m => m.TeacherId == id);

            IQueryable<Course> coursesQuery = _context.Course.Where(m => m.FirstTeacherId == id || m.SecondTeacherId == id);
            await _context.SaveChangesAsync();
            if (teacher == null)
            {
                return NotFound();
            }
            ViewBag.Message = teacher.FirstName;
            var courseVM = new CourseViewModel
            {
                Courses = await coursesQuery.ToListAsync(),
            };

            return View(courseVM);
        }
    }
}


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
    public class EnrollmentsController : Controller
    {
        private readonly MVCSchoolAppContext _context;

        public EnrollmentsController(MVCSchoolAppContext context)
        {
            _context = context;
        }

        // GET: Enrollments
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var mVCSchoolAppContext = _context.Enrollment.Include(e => e.Course).Include(e => e.Student);
            return View(await mVCSchoolAppContext.ToListAsync());
        }

        // GET: Enrollments/Details/5
        [Authorize]
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment
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
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Course, "CourseId", "Title");
            ViewData["StudentId"] = new SelectList(_context.Student, "StudentId", "FirstName");
            return View();
        }

        // POST: Enrollments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("EnrollmentId,CourseId,StudentId,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoints,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(enrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "CourseId", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "StudentId", "FirstName", enrollment.StudentId);
            return View(enrollment);
        }

        // GET: Enrollments/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "CourseId", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "StudentId", "FirstName", enrollment.StudentId);
            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(long id, [Bind("EnrollmentId,CourseId,StudentId,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoints,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            if (id != enrollment.EnrollmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
            ViewData["CourseId"] = new SelectList(_context.Course, "CourseId", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "StudentId", "FirstName", enrollment.StudentId);
            return View(enrollment);
        }

        // GET: Enrollments/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var enrollment = await _context.Enrollment.FindAsync(id);
            _context.Enrollment.Remove(enrollment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EnrollmentExists(long id)
        {
            return _context.Enrollment.Any(e => e.EnrollmentId == id);
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> TeacherFunctions(int id, string teacher, int year)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course.FirstOrDefaultAsync(m => m.CourseId == id);

            var teacherModel = await _context.Teacher.FirstOrDefaultAsync(m => m.FirstName == teacher);
            ViewBag.teacher = teacher;
            ViewBag.course = course;
            var enrollment = _context.Enrollment.Where(x => x.CourseId == id && (x.Course.FirstTeacherId == teacherModel.TeacherId || x.Course.SecondTeacherId == teacherModel.TeacherId))
                .Include(e => e.Course)
                .Include(e => e.Student);
            await _context.SaveChangesAsync();
            IQueryable<int?> yearsQuery = _context.Enrollment.OrderBy(m => m.Year).Select(m => m.Year).Distinct();
            IQueryable<Enrollment> enrollmentQuery = enrollment.AsQueryable();
            if (year != null && year != 0)
            {
                enrollmentQuery = enrollmentQuery.Where(x => x.Year == year);
            }
            else
            {
                enrollmentQuery = enrollmentQuery.Where(x => x.Year == yearsQuery.Max());
            }

            if (enrollment == null)
            {
                return NotFound();
            }

            var Enrollments = await enrollmentQuery.ToListAsync();
            var ocnds = new SelectList(await yearsQuery.ToListAsync());

            TeacherStudent viewmodel = new TeacherStudent
            {
               
                Enrolls = await enrollmentQuery.ToListAsync(),
                YearList = new SelectList(await yearsQuery.ToListAsync())
            };

            return View(viewmodel);
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> TeacherEdit(long? id, string teacher)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }

            ViewBag.teacher = teacher;
            ViewData["CourseId"] = new SelectList(_context.Course, "CourseId", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "StudentId", "FirstName", enrollment.StudentId);
            return View(enrollment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> TeacherEdit(int id, string teacher, [Bind("EnrollmentId,CourseId,StudentId,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoint,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            if (id != enrollment.EnrollmentId)
            {
                return NotFound();
            }
            string pom = teacher;
            if (ModelState.IsValid)
            {
                try
                {
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
                return RedirectToAction("Professor", new { CourseId = enrollment.CourseId, Teacher = pom, Year = enrollment.Year });
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "CourseId", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "CourseId", "FirstName", enrollment.StudentId);
            return View(enrollment);
        }

        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> StudentFunctions(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.StudentId == id);

            ViewBag.student = student.FirstName;

            IQueryable<Enrollment> enrollment = _context.Enrollment.Where(x => x.StudentId == id)
            .Include(e => e.Course)
            .Include(e => e.Student);
            await _context.SaveChangesAsync();

            if (enrollment == null)
            {
                return NotFound();
            }

            return View(await enrollment.ToListAsync());
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = _context.Enrollment.Where(m => m.EnrollmentId == id).Include(x => x.Student).Include(x => x.Course).First();
            IQueryable<Enrollment> enrollmentQuery = _context.Enrollment.AsQueryable();
            enrollmentQuery = enrollmentQuery.Where(m => m.EnrollmentId == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            StudentViewModel viewmodel = new StudentViewModel
            {
                Enrollment = await enrollmentQuery.Include(x => x.Student).Include(x => x.Course).FirstAsync(),
                SeminalUrlName = enrollment.SeminalUrl
            };
            ViewData["CourseId"] = new SelectList(_context.Course, "CourseId", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "StudentId", "FirstName", enrollment.StudentId);
            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentEdit(long id, StudentViewModel viewmodel)
        {
            if (id != viewmodel.Enrollment.EnrollmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    if (viewmodel.SeminalUrlFile != null)
                    {
                        string uniqueFileName = UploadedFile(viewmodel);
                        viewmodel.Enrollment.SeminalUrl = uniqueFileName;
                    }
                    else
                    {
                        viewmodel.Enrollment.SeminalUrl = viewmodel.SeminalUrlName;
                    }

                    _context.Update(viewmodel.Enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(viewmodel.Enrollment.EnrollmentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("StudentFunctions", new { id = viewmodel.Enrollment.StudentId });
            }

            ViewData["CourseId"] = new SelectList(_context.Course, "CourseId", "Title", viewmodel.Enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "StudentId", "FirstName", viewmodel.Enrollment.StudentId);
            return View(viewmodel);
        }

        private string UploadedFile(StudentViewModel viewmodel)
        {
            string uniqueFileName = null;

            if (viewmodel.SeminalUrlFile != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/urls");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(viewmodel.SeminalUrlFile.FileName);
                string fileNameWithPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewmodel.SeminalUrlFile.CopyTo(stream);
                }
            }
            return uniqueFileName;
        }
    }
}

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
    public class TeachersController : Controller
    {
        private readonly MVCSchoolAppContext _context;

        public TeachersController(MVCSchoolAppContext context)
        {
            _context = context;
        }

        // GET: Teachers
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string TeacherFName, string TeacherLName, string TeacherAcRank, string TeacherEdLevel)
        {
            IQueryable<Teacher> teachers = _context.Teacher.AsQueryable();
            IQueryable<string> rankQuery = _context.Teacher.OrderBy(m => m.AcademicRank).Select(m => m.AcademicRank).Distinct();

            if (!string.IsNullOrEmpty(TeacherFName) && (!string.IsNullOrEmpty(TeacherLName)))
            {
                teachers = teachers.Where(c => c.FirstName.Contains(TeacherFName)).Where(c => c.LastName.Contains(TeacherLName));
            }

            else if (!string.IsNullOrEmpty(TeacherFName) && (string.IsNullOrEmpty(TeacherLName)))
            {
                teachers = teachers.Where(c => c.FirstName.Contains(TeacherFName));
            }



            else if (string.IsNullOrEmpty(TeacherFName) && (!string.IsNullOrEmpty(TeacherLName)))
            {
                teachers = teachers.Where(c => c.LastName.Contains(TeacherLName));
            }

            if (!string.IsNullOrEmpty(TeacherEdLevel))
            {
                teachers = teachers.Where(c => c.Degree.Contains(TeacherEdLevel));
            }


            if (!string.IsNullOrEmpty(TeacherAcRank))
            {
                teachers = teachers.Where(c => c.AcademicRank == TeacherAcRank);
            }

            teachers = teachers.Include(c => c.CoursesAsFirstTeacher)
                               .Include(c => c.CoursesAsSecondTeacher);

            var teacherfilterVM = new TeacherFilter
            {
                filteredT = new SelectList(await rankQuery.ToListAsync()),
                Teachers = await teachers.ToListAsync()
            };
            return View(teacherfilterVM);
        }

        // GET: Teachers/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher.Include(c => c.CoursesAsFirstTeacher)
                .Include(c => c.CoursesAsSecondTeacher)
                .FirstOrDefaultAsync(m => m.TeacherId == id);
            if (teacher == null)
            {
                return NotFound();
            }

            TeacherImageVM viewmodel = new TeacherImageVM
            {
                teacher = teacher,
                ProfileImageName = teacher.ProfilePicture
            };

            return View(viewmodel);
        }

        // GET: Teachers/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teachers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("TeacherId,FirstName,LastName,Degree,AcademicRank,OfficeNumber,HireDate")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                _context.Add(teacher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
        }

        // GET: Teachers/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            return View(teacher);
        }

        // POST: Teachers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("TeacherId,FirstName,LastName,Degree,AcademicRank,OfficeNumber,HireDate")] Teacher teacher)
        {
            if (id != teacher.TeacherId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teacher.TeacherId))
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
            return View(teacher);
        }

        // GET: Teachers/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher
                .FirstOrDefaultAsync(m => m.TeacherId == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teacher.FindAsync(id);
            _context.Teacher.Remove(teacher);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeacherExists(int id)
        {
            return _context.Teacher.Any(e => e.TeacherId == id);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditPicture(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = _context.Teacher.Where(x => x.TeacherId == id).First();
            if (teacher == null)
            {
                return NotFound();
            }

            TeacherImageVM viewmodel = new TeacherImageVM
            {
                teacher = teacher,
                ProfileImageName = teacher.ProfilePicture
            };

            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditPicture(long id, TeacherImageVM viewmodel)
        {
            if (id != viewmodel.teacher.TeacherId)
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
                        viewmodel.teacher.ProfilePicture = uniqueFileName;
                    }
                    else
                    {
                        viewmodel.teacher.ProfilePicture = viewmodel.ProfileImageName;
                    }

                    _context.Update(viewmodel.teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(viewmodel.teacher.TeacherId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { id = viewmodel.teacher.TeacherId });
            }
            return View(viewmodel);
        }

        private string UploadedFile(TeacherImageVM model)
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

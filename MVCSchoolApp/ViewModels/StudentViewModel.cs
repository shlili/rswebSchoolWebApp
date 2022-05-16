using MVCSchoolApp.Models;
using System.ComponentModel.DataAnnotations;

namespace MVCSchoolApp.ViewModels
{
    public class StudentViewModel
    {
        public Enrollment Enrollment { get; set; }

        [Display(Name = "Seminal Project File")]
        public IFormFile? SeminalUrlFile { get; set; }
        public string? SeminalUrlName { get; set; }
    }
}

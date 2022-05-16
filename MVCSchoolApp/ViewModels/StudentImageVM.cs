using MVCSchoolApp.Models;
using System.ComponentModel.DataAnnotations;

namespace MVCSchoolApp.ViewModels
{
    public class StudentImageVM
    {
        public Student? student { get; set; }

        [Display(Name = "Upload picture")]
        public IFormFile? ProfileImage { get; set; }

        [Display(Name = "Picture name")]
        public string? ProfileImageName { get; set; }
    }
}

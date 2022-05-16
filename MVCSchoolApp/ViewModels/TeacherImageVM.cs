using MVCSchoolApp.Models;
using System.ComponentModel.DataAnnotations;

namespace MVCSchoolApp.ViewModels
{
    public class TeacherImageVM
    {
        public Teacher? teacher { get; set; }

        [Display(Name = "Upload picture")]
        public IFormFile? ProfileImage { get; set; }

        [Display(Name = "Picture name")]
        public string? ProfileImageName { get; set; }
    }
}

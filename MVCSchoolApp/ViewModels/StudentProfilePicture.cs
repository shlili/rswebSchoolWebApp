using MVCSchoolApp.Models;
using System.ComponentModel.DataAnnotations;

namespace MVCSchoolApp.ViewModels
{
    public class StudentProfilePicture
    {
        public Student Student { get; set; }

        [Display(Name = "Upload")]
        public IFormFile ProfilePictureFile { get; set; }

        [Display(Name = "Picture")]
        public string ProfilePictureName { get; set; }
    }
}

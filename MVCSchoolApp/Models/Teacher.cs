using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MVCSchoolApp.Models;

namespace MVCSchoolApp.Models
{
    public class Teacher
    {
        [Required]
        public int TeacherId { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Required]
        public string FirstName { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Required]
        public string LastName { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }

        [Display(Name = "Degree")]
        [StringLength(50)]
        public string? Degree { get; set; }

        [Display(Name = "Academic Rank")]
        [StringLength(50)]
        public string? AcademicRank { get; set; }

        [Display(Name = "Office Number")]
        [StringLength(10)]
        public string? OfficeNumber { get; set; }

        [Display(Name = "Hire Date")]
        [DataType(DataType.Date)]
        public DateTime? HireDate { get; set; }

        public string? ProfilePicture { get; set; }

        [InverseProperty(nameof(Course.FirstTeacher))]
        public ICollection<Course>? CoursesAsFirstTeacher { get; set; }

        [InverseProperty(nameof(Course.SecondTeacher))]
        public ICollection<Course>? CoursesAsSecondTeacher { get; set; }
    }
}

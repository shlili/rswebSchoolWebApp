using System.ComponentModel.DataAnnotations;

namespace MVCSchoolApp.Models
{
    public class Student
    {
        [Required]
        public long StudentId { get; set; }

        [Display(Name = "First Name")]
        [StringLength(10, MinimumLength = 3)]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(50, MinimumLength = 5)]
        [Required]
        public string LastName { get; set; }
        [Display(Name = "Full Name")]
        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }

        [Display(Name = "Date of enrollment")]
        [DataType(DataType.Date)]
        public DateTime EnrollmentDate { get; set; }

        [Display(Name = "Acquired Credits")]
        public int AcquiredCredits { get; set; }

        [Display(Name = "Current Semester")]
        public int CurrentSemester { get; set; }

        [Display(Name = "Education Level")]
        [StringLength(25)]
        public string EducationLevel { get; set; }

        public string? ProfilePicture { get; set; }

        public ICollection<Enrollment>? Courses { get; set; }
    }
}

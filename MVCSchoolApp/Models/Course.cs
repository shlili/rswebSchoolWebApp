using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MVCSchoolApp.Models;

namespace MVCSchoolApp.Models
{
    public class Course
    {
        [Required]
        public int CourseId { get; set; }

        [StringLength(100, MinimumLength = 3)]
        [Required]
        public string Title { get; set; }

        [Required]
        public int? Credits { get; set; }

        public int Semester { get; set; }

        [StringLength(100)]
        public string Programme { get; set; }

        [Display(Name = "Education Level")]
        [StringLength(25)]
        public string EducationLevel { get; set; }

        [ForeignKey(nameof(FirstTeacher)), Column(Order = 0)]
        public int? FirstTeacherId { get; set; }

        [ForeignKey(nameof(SecondTeacher)), Column(Order = 1)]
        public int? SecondTeacherId { get; set; }

        [Display(Name = "Professor")]
        public Teacher FirstTeacher { get; set; }

        [Display(Name = "Asisstant")]
        public Teacher SecondTeacher { get; set; }

        public ICollection<Enrollment> Students { get; set; }
    }
}

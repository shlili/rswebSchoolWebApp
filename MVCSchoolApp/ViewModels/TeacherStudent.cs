using Microsoft.AspNetCore.Mvc.Rendering;
using MVCSchoolApp.Models;

namespace MVCSchoolApp.ViewModels
{
    public class TeacherStudent
    {
        public List<Enrollment> Enrolls { get; set; }
        public SelectList YearList { get; set; }
        public int? Year { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using MVCSchoolApp.Models;

namespace MVCSchoolApp.ViewModels
{
    public class StudentViewModel2
    {
        public List<Student> Students { get; set; }
        public SelectList IDs { get; set; }
        public int Indexes { get; set; }
    }
}

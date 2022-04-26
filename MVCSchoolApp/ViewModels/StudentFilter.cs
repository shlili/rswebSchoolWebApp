using Microsoft.AspNetCore.Mvc.Rendering;
using MVCSchoolApp.Models;

namespace MVCSchoolApp.ViewModels
{
    public class StudentFilter
    {
        public IList<Student> Students { get; set; }
        public SelectList filteredS { get; set; }
        public long StudentIndex { get; set; }
        public string StudentFName { get; set; }
        public string StudentLName { get; set; }
    }
}

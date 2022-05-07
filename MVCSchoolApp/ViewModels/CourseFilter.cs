using Microsoft.AspNetCore.Mvc.Rendering;
using MVCSchoolApp.Models;
using System.Collections.Generic;

namespace MVCSchoolApp.ViewModels
{
    public class CourseFilter
    {
        public IList<Course> Courses { get; set; }
        public SelectList filtered { get; set; }
        public int CourseSem { get; set; }
        public string CourseProg { get; set; }
        public string CTitle { get; set; }
    }
}

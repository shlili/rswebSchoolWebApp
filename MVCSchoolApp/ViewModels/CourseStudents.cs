using Microsoft.AspNetCore.Mvc.Rendering;
using MVCSchoolApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCSchoolApp.ViewModels
{
    public class CourseStudents
    {
        public Course? Course { get; set; }
        public IEnumerable<long>? SelStudents { get; set; }
        public IEnumerable<SelectListItem>? StudentsList { get; set; }
        public string? ProjectUrl { get; set; }
    }
}

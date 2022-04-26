using Microsoft.AspNetCore.Mvc.Rendering;
using MVCSchoolApp.Models;
using System.Collections.Generic;


namespace MVCSchoolApp.ViewModels
{
    public class TeacherFilter
    {
        public IList<Teacher> Teachers { get; set; }
        public SelectList filteredT { get; set; }
        public string TeacherAcRank { get; set; }
        public string TeacherFName { get; set; }
        public string TeacherLName { get; set; }
        public string TeacherEdLevel { get; set; }
    }
}

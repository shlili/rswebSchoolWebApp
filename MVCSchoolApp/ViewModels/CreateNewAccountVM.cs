using Microsoft.AspNetCore.Mvc.Rendering;
using MVCSchoolApp.Models;

namespace MVCSchoolApp.ViewModels
{
    public class CreateNewAccountVM
    {
        public IEnumerable<SelectListItem>? TeacherList { get; set; }

        public IEnumerable<int>? SelectedTeacher { get; set; }

    }
}

using Microsoft.EntityFrameworkCore;
using MVCSchoolApp.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCSchoolApp.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MVCSchoolAppContext(
            serviceProvider.GetRequiredService<
            DbContextOptions<MVCSchoolAppContext>>()))
            {
                // Look for any movies.
                if (!context.Student.Any())
                {
                    context.Student.AddRange(
               new Student { /*Id = 1, */FirstName = "Billy", LastName = "Crystal", EnrollmentDate = DateTime.Parse("1948-3-14"), AcquiredCredits = 30, CurrentSemester = 3, EducationLevel = "HS Graduate" },
               new Student { /*Id = 2, */FirstName = "Meg", LastName = "Ryan", EnrollmentDate = DateTime.Parse("1961-11-19"), AcquiredCredits = 60, CurrentSemester = 3, EducationLevel = "HS Graduate" },
               new Student { /*Id = 3, */FirstName = "Carrie", LastName = "Fisher", EnrollmentDate = DateTime.Parse("1956-10-21"), AcquiredCredits = 156, CurrentSemester = 7, EducationLevel = "HS Graduate" },
               new Student { /*Id = 4, */FirstName = "Bill", LastName = "Murray", EnrollmentDate = DateTime.Parse("1950-9-21"), AcquiredCredits = 15, CurrentSemester = 1, EducationLevel = "HS Graduate" },
               new Student { /*Id = 5, */FirstName = "Dan", LastName = "Aykroyd", EnrollmentDate = DateTime.Parse("1952-7-1"), AcquiredCredits = 76, CurrentSemester = 5, EducationLevel = "HS Graduate" },
               new Student { /*Id = 6, */FirstName = "Sigourney", LastName = "Weaver", EnrollmentDate = DateTime.Parse("1949-11-8"), AcquiredCredits = 27, CurrentSemester = 1, EducationLevel = "HS Graduate" },
               new Student { /*Id = 7, */FirstName = "John", LastName = "Wayne", EnrollmentDate = DateTime.Parse("1907-5-26"), AcquiredCredits = 97, CurrentSemester = 5, EducationLevel = "HS Graduate" },
               new Student { /*Id = 8, */FirstName = "Dean", LastName = "Martin", EnrollmentDate = DateTime.Parse("1917-6-7"), AcquiredCredits = 53, CurrentSemester = 3, EducationLevel = "HS Graduate" }
               );
                    context.SaveChanges();
                }
                if (!context.Teacher.Any())
                {
                    context.Teacher.AddRange(
                new Teacher { /*Id = 1, */FirstName = "Rob", LastName = "Reiner", HireDate = DateTime.Parse("1947-3-6"), AcademicRank = "3", OfficeNumber = "one", Degree = "PhD" },
                new Teacher { /*Id = 2, */FirstName = "Ivan", LastName = "Reitman", HireDate = DateTime.Parse("1946-11-27"), AcademicRank = "3", OfficeNumber = "two", Degree = "PhD" },
                new Teacher { /*Id = 3, */FirstName = "Howard", LastName = "Hawks", HireDate = DateTime.Parse("1896-5-30"), AcademicRank = "2", OfficeNumber = "three", Degree = "MsC" },
                new Teacher { /*Id = 1, */FirstName = "Michael", LastName = "Scott", HireDate = DateTime.Parse("1949-1-1"), AcademicRank = "3", OfficeNumber = "four", Degree = "PhD" },
                new Teacher { /*Id = 2, */FirstName = "Glenn", LastName = "Sturgis", HireDate = DateTime.Parse("1950-9-21"), AcademicRank = "2", OfficeNumber = "two", Degree = "MsC" },
                new Teacher { /*Id = 3, */FirstName = "Sheldon", LastName = "Cooper", HireDate = DateTime.Parse("1888-4-19"), AcademicRank = "3", OfficeNumber = "four", Degree = "PhD" }
                );
                    _ = context.SaveChanges();
                }
                if (!context.Course.Any())
                {
                    context.Course.AddRange(
                new Course
                {
                    //Id = 1,
                    Title = "Mathematics 1",
                    Programme = "all",
                    Credits = 7,
                    Semester = 1,
                    FirstTeacherId = context.Teacher.Single(d => d.FirstName == "Michael" && d.LastName == "Scott").TeacherId,
                    EducationLevel = "BS"
                },
                new Course
                {
                    Title = "Logical Design",
                    Programme = "KHIE",
                    Credits = 7,
                    Semester = 3,
                    EducationLevel = "MS",
                    FirstTeacherId = context.Teacher.Single(d => d.FirstName == "Sheldon" && d.LastName == "Cooper").TeacherId,
                    SecondTeacherId = context.Teacher.Single(d => d.FirstName == "Glenn" && d.LastName == "Sturgis").TeacherId
                },
                new Course
                {
                    Title = "RSWeb",
                    Programme = "KTI",
                    Credits = 7,
                    Semester = 5,
                    EducationLevel = "BS",
                    FirstTeacherId = context.Teacher.Single(d => d.FirstName == "Michael" && d.LastName == "Scott").TeacherId,
                    SecondTeacherId = context.Teacher.Single(d => d.FirstName == "Glenn" && d.LastName == "Sturgis").TeacherId
                },
                new Course
                {
                    Title = "Physics",
                    Programme = "all",
                    Credits = 7,
                    Semester = 2,
                    EducationLevel = "BS",
                    FirstTeacherId = context.Teacher.Single(d => d.FirstName == "Sheldon" && d.LastName == "Cooper").TeacherId,
                    SecondTeacherId = context.Teacher.Single(d => d.FirstName == "Michael" && d.LastName == "Scott").TeacherId
                }
                );
                    context.SaveChanges();
                }
                
               if (!context.Enrollment.Any())
                {
                    context.Enrollment.AddRange
                (
                new Enrollment { StudentId = 1, CourseId = 6},
                new Enrollment { StudentId = 2, CourseId = 6},
                new Enrollment { StudentId = 3, CourseId = 6},
                new Enrollment { StudentId = 4, CourseId = 5},
                new Enrollment { StudentId = 5, CourseId = 5},
                new Enrollment { StudentId = 6, CourseId = 5},
                new Enrollment { StudentId = 4, CourseId = 3},
                new Enrollment { StudentId = 5, CourseId = 3},
                new Enrollment { StudentId = 6, CourseId = 3},
                new Enrollment { StudentId = 7, CourseId = 4},
                new Enrollment { StudentId = 8, CourseId = 4}
                );
                    context.SaveChanges();
                }
                
                
            }
        }

    }
}

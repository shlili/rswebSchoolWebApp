#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MVCSchoolApp.Models;

namespace MVCSchoolApp.Data
{
    public class MVCSchoolAppContext : DbContext
    {
        public MVCSchoolAppContext (DbContextOptions<MVCSchoolAppContext> options)
            : base(options)
        {
        }

        public DbSet<MVCSchoolApp.Models.Student> Student { get; set; }

        public DbSet<MVCSchoolApp.Models.Teacher> Teacher { get; set; }

        public DbSet<MVCSchoolApp.Models.Course> Course { get; set; }

        public DbSet<Enrollment> Enrollment{ get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Enrollment>()
            .HasOne<Student>(p => p.Student)
            .WithMany(p => p.Courses)
            .HasForeignKey(p => p.StudentId);
            //.HasPrincipalKey(p => p.Id);
            builder.Entity<Enrollment>()
            .HasOne<Course>(p => p.Course)
            .WithMany(p => p.Students)
            .HasForeignKey(p => p.CourseId);
            //.HasPrincipalKey(p => p.Id);
        }
    }
}

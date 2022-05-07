﻿// <auto-generated />
using System;
using MVCSchoolApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MVCSchoolApp.Migrations
{
    [DbContext(typeof(MVCSchoolAppContext))]
    [Migration("20220507152858_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("MVCSchoolApp.Models.Course", b =>
                {
                    b.Property<int>("CourseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CourseId"), 1L, 1);

                    b.Property<int?>("Credits")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<string>("EducationLevel")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.Property<int?>("FirstTeacherId")
                        .HasColumnType("int");

                    b.Property<string>("Programme")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int?>("SecondTeacherId")
                        .HasColumnType("int");

                    b.Property<int?>("Semester")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("CourseId");

                    b.HasIndex("FirstTeacherId");

                    b.HasIndex("SecondTeacherId");

                    b.ToTable("Course");
                });

            modelBuilder.Entity("MVCSchoolApp.Models.Enrollment", b =>
                {
                    b.Property<long>("EnrollmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("EnrollmentId"), 1L, 1);

                    b.Property<int>("AdditionalPoints")
                        .HasColumnType("int");

                    b.Property<int>("CourseId")
                        .HasColumnType("int");

                    b.Property<int>("ExamPoints")
                        .HasColumnType("int");

                    b.Property<DateTime>("FinishDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Grade")
                        .HasColumnType("int");

                    b.Property<int>("ProjectPoints")
                        .HasColumnType("int");

                    b.Property<string>("ProjectUrl")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Semester")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<int>("SeminalPoints")
                        .HasColumnType("int");

                    b.Property<string>("SeminalUrl")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<long>("StudentId")
                        .HasColumnType("bigint");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("EnrollmentId");

                    b.HasIndex("CourseId");

                    b.HasIndex("StudentId");

                    b.ToTable("Enrollment");
                });

            modelBuilder.Entity("MVCSchoolApp.Models.Student", b =>
                {
                    b.Property<long>("StudentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("StudentId"), 1L, 1);

                    b.Property<int>("AcquiredCredits")
                        .HasColumnType("int");

                    b.Property<int>("CurrentSemester")
                        .HasColumnType("int");

                    b.Property<string>("EducationLevel")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.Property<DateTime>("EnrollmentDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("StudentId");

                    b.ToTable("Student");
                });

            modelBuilder.Entity("MVCSchoolApp.Models.Teacher", b =>
                {
                    b.Property<int>("TeacherId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TeacherId"), 1L, 1);

                    b.Property<string>("AcademicRank")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Degree")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("HireDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("OfficeNumber")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("TeacherId");

                    b.ToTable("Teacher");
                });

            modelBuilder.Entity("MVCSchoolApp.Models.Course", b =>
                {
                    b.HasOne("MVCSchoolApp.Models.Teacher", "FirstTeacher")
                        .WithMany("CoursesAsFirstTeacher")
                        .HasForeignKey("FirstTeacherId");

                    b.HasOne("MVCSchoolApp.Models.Teacher", "SecondTeacher")
                        .WithMany("CoursesAsSecondTeacher")
                        .HasForeignKey("SecondTeacherId");

                    b.Navigation("FirstTeacher");

                    b.Navigation("SecondTeacher");
                });

            modelBuilder.Entity("MVCSchoolApp.Models.Enrollment", b =>
                {
                    b.HasOne("MVCSchoolApp.Models.Course", "Course")
                        .WithMany("Students")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MVCSchoolApp.Models.Student", "Student")
                        .WithMany("Courses")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("MVCSchoolApp.Models.Course", b =>
                {
                    b.Navigation("Students");
                });

            modelBuilder.Entity("MVCSchoolApp.Models.Student", b =>
                {
                    b.Navigation("Courses");
                });

            modelBuilder.Entity("MVCSchoolApp.Models.Teacher", b =>
                {
                    b.Navigation("CoursesAsFirstTeacher");

                    b.Navigation("CoursesAsSecondTeacher");
                });
#pragma warning restore 612, 618
        }
    }
}
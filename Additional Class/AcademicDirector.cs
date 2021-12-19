using LCCS_School_Parent_Communication_System.Identity;
using LCCS_School_Parent_Communication_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LCCS_School_Parent_Communication_System.viewModels;
using Microsoft.AspNet.Identity;

namespace LCCS_School_Parent_Communication_System.Additional_Class
{
    public class AcademicDirector
    {

        //function to record teacher information
        public void registerTeacher(Teacher teacher)
        {
            //record teacher information
            ApplicationDbContext context = new ApplicationDbContext();
            context.Teacher.Add(teacher);
            context.SaveChanges();
        }

        //function to update user information
        public void UpdateTeacher(Teacher teacher)
        {
            //update teacher with the new information
            ApplicationDbContext context = new ApplicationDbContext();
            Teacher teacherUP = new Teacher(1);

            //find the target user ising teacher id
            teacherUP = context.Teacher.Find(teacher.teacherId);

            //update teacher information
            teacherUP.subject = teacher.subject;
            teacherUP.user.fullName = teacher.user.fullName;
            teacherUP.grade = teacher.grade;
            context.SaveChanges();
        }

        //function to delete teacher information
        public void DeleteTeacher(string id)
        {
            //delete teacher record using teacher id
            ApplicationDbContext context = new ApplicationDbContext();
            Teacher teacher = new Teacher();

            //delete teacher record
            teacher = context.Teacher.Find(id);
            context.Teacher.Remove(teacher);
            context.SaveChanges();
        }

        //function to validate the durations specified by the academic director
        public Boolean validateDuration(AcademicYearViewModel academicYearViewModel)
        {
            //check if start an d end dates are not the same
            if(DateTime.Compare(academicYearViewModel.yearStart,academicYearViewModel.yearEnd)!=0 && DateTime.Compare(academicYearViewModel.quarterOneStart, academicYearViewModel.quarterOneEnd) != 0 &&
                DateTime.Compare(academicYearViewModel.quarterTwoStart, academicYearViewModel.quarterTwoEnd) != 0 && DateTime.Compare(academicYearViewModel.quarterThreeStart, academicYearViewModel.quarterThreeEnd) != 0 &&
                DateTime.Compare(academicYearViewModel.quarterFourStart, academicYearViewModel.quarterFourEnd) != 0)
            {
                //check if start dates are earlier than the end date
                if(DateTime.Compare(academicYearViewModel.quarterOneStart, academicYearViewModel.quarterOneEnd) < 0 && DateTime.Compare(academicYearViewModel.quarterTwoStart, academicYearViewModel.quarterTwoEnd) < 0 &&
                    DateTime.Compare(academicYearViewModel.quarterThreeStart, academicYearViewModel.quarterThreeEnd) < 0 && DateTime.Compare(academicYearViewModel.quarterFourStart, academicYearViewModel.quarterFourEnd) < 0 &&
                    DateTime.Compare(academicYearViewModel.yearStart, academicYearViewModel.yearEnd) < 0)
                {
                    //check if the dates dont overlap and quarter dates are within the year boundary
                    if(DateTime.Compare(academicYearViewModel.quarterOneEnd, academicYearViewModel.quarterTwoStart) < 0 && DateTime.Compare(academicYearViewModel.quarterTwoEnd, academicYearViewModel.quarterThreeStart) < 0 &&
                    DateTime.Compare(academicYearViewModel.quarterThreeEnd, academicYearViewModel.quarterFourStart) < 0 && (DateTime.Compare(academicYearViewModel.quarterFourEnd, academicYearViewModel.yearEnd) < 0 || DateTime.Compare(academicYearViewModel.quarterFourEnd, academicYearViewModel.yearEnd) == 0))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public SectionViewModel populateFormData()
        {
            //basic objects
            SectionViewModel sectionViewModel = new SectionViewModel();
            AcademicYearViewModel academicYearViewModel = new AcademicYearViewModel();
            ApplicationDbContext context = new ApplicationDbContext();
            ApplicationUserStore userStore = new ApplicationUserStore(context);
            ApplicationUserManager userManager = new ApplicationUserManager(userStore);
            sectionViewModel.teachers = new List<string>();
            sectionViewModel.academicYears = new List<string>();
            sectionViewModel.letter = new List<char>();
            Teacher teacher = new Teacher();
            AcademicYear academicYear = new AcademicYear();

            //retrive all teachers that have teacher role from teacher table and populate it to sectionViewModel
            var allTeachers = context.Teacher.ToList();

            foreach (var getNames in allTeachers)
            {
                if (userManager.IsInRole(getNames.teacherId, "Teacher"))
                {
                    sectionViewModel.teachers.Add(getNames.user.fullName);
                }
            }

            //retrive all Academic Years that are already active and populate it to sectionViewModel
            var allAcadamicYears = context.AcademicYear.ToList();
            foreach (var getAcadamicYear in allAcadamicYears)
            {
                string[] duration = getAcadamicYear.duration.Split('-');
                if (!(DateTime.Compare(DateTime.Now, DateTime.Parse(duration[0])) < 0 || DateTime.Compare(DateTime.Now, DateTime.Parse(duration[1])) > 0))
                {
                    sectionViewModel.academicYears.Add(getAcadamicYear.academicYearName);
                }
            }

            //populate section letters with alphabet letters
            for (char c = 'A'; c <= 'Z'; c++)
            {
                sectionViewModel.letter.Add(c);
            }

            return sectionViewModel;
        }

        public SectionViewModel searchSection(int grade,string letter)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var allAcadamicYears = context.AcademicYear.ToList();
            var sectionViewModel = new SectionViewModel();
            sectionViewModel.teachers = new List<string>();
            sectionViewModel.academicYears = new List<string>();
            foreach (var getAcadamicYear in allAcadamicYears)
            {
                string[] duration = getAcadamicYear.duration.Split('-');
                if (!(DateTime.Compare(DateTime.Now, DateTime.Parse(duration[0])) < 0 || DateTime.Compare(DateTime.Now, DateTime.Parse(duration[1])) > 0))
                {
                    var sectionRecord = context.Section.Where(s => s.sectionName == grade.ToString() + letter && s.academicYearId==getAcadamicYear.academicYearName).ToList();

                    if (sectionRecord.Count != 0)
                    {
                        foreach (var data in sectionRecord)
                        {
                            sectionViewModel.teachers.Add(data.teacher.user.fullName);
                            sectionViewModel.academicYears.Add(data.academicYearId);
                            break;
                        }
                        return sectionViewModel;
                    }
                }
            }

            return null;
        }





        public RegistrarManagementViewModel listRegistrar()
        {
            //function for listing every user with registrar role 
            var appDbContext = new ApplicationDbContext();
            var userStore = new ApplicationUserStore(appDbContext);
            var userManager = new ApplicationUserManager(userStore);
            ApplicationUser user = new ApplicationUser();
            RegistrarManagementViewModel rvm = new RegistrarManagementViewModel();

            List<ApplicationUser> users = new List<ApplicationUser>();
            users = appDbContext.Users.ToList();

            foreach (var k in users)
            {
                if (userManager.IsInRole(k.Id, "Registrar"))
                {
                    rvm.registrarList.Add(new ApplicationUser
                    {
                        Id = k.Id,
                        fullName = k.fullName,
                        UserName = k.UserName,
                        Email = k.Email

                    });
                }

            }

           
            return rvm;
        }
    }
}
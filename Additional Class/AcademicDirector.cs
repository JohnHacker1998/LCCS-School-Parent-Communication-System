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

        public Boolean validateDuration(AcademicYearViewModel academicYearViewModel)
        {

            if(DateTime.Compare(academicYearViewModel.yearStart,academicYearViewModel.yearEnd)!=0 && DateTime.Compare(academicYearViewModel.quarterOneStart, academicYearViewModel.quarterOneEnd) != 0 &&
                DateTime.Compare(academicYearViewModel.quarterTwoStart, academicYearViewModel.quarterTwoEnd) != 0 && DateTime.Compare(academicYearViewModel.quarterThreeStart, academicYearViewModel.quarterThreeEnd) != 0 &&
                DateTime.Compare(academicYearViewModel.quarterFourStart, academicYearViewModel.quarterFourEnd) != 0)
            {
                if(DateTime.Compare(academicYearViewModel.quarterOneStart, academicYearViewModel.quarterOneEnd) < 0 && DateTime.Compare(academicYearViewModel.quarterTwoStart, academicYearViewModel.quarterTwoEnd) < 0 &&
                    DateTime.Compare(academicYearViewModel.quarterThreeStart, academicYearViewModel.quarterThreeEnd) < 0 && DateTime.Compare(academicYearViewModel.quarterFourStart, academicYearViewModel.quarterFourEnd) < 0 &&
                    DateTime.Compare(academicYearViewModel.yearStart, academicYearViewModel.yearEnd) < 0)
                {
                    if(DateTime.Compare(academicYearViewModel.quarterOneEnd, academicYearViewModel.quarterTwoStart) < 0 && DateTime.Compare(academicYearViewModel.quarterTwoEnd, academicYearViewModel.quarterThreeStart) < 0 &&
                    DateTime.Compare(academicYearViewModel.quarterThreeEnd, academicYearViewModel.quarterFourStart) < 0 && (DateTime.Compare(academicYearViewModel.quarterFourEnd, academicYearViewModel.yearEnd) < 0 || DateTime.Compare(academicYearViewModel.quarterFourEnd, academicYearViewModel.yearEnd) == 0))
                    {
                        return true;
                    }
                }
            }

            return false;
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
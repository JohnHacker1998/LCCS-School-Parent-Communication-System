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

        public Boolean validateDuration(AcademicYearViewModel ayvm)
        {
            DateTime yearStartOne = Convert.ToDateTime(ayvm.yearStart);
            DateTime yearEndOne = Convert.ToDateTime(ayvm.yearEnd);
            DateTime zquarterOneStart = Convert.ToDateTime(ayvm.quarterOneStart);
            DateTime zquarterOneEnd = Convert.ToDateTime(ayvm.quarterOneEnd);
            DateTime zquarterTwoStart = Convert.ToDateTime(ayvm.quarterTwoStart);
            DateTime zquarterTwoEnd = Convert.ToDateTime(ayvm.quarterTwoEnd);
            DateTime zquarterThreeStart = Convert.ToDateTime(ayvm.quarterThreeStart);
            DateTime zquarterThreeEnd = Convert.ToDateTime(ayvm.quarterThreeEnd);
            DateTime zquarterFourStart = Convert.ToDateTime(ayvm.quarterFourStart);
            DateTime zquarterFourEnd = Convert.ToDateTime(ayvm.quarterFourEnd);

            if (DateTime.Compare(yearStartOne, yearEndOne) != 0 && DateTime.Compare(zquarterOneStart, zquarterOneEnd) != 0 &&
                DateTime.Compare(zquarterTwoStart, zquarterTwoEnd) != 0 && DateTime.Compare(zquarterThreeStart, zquarterThreeEnd) != 0 &&
                DateTime.Compare(zquarterFourStart, zquarterFourEnd) != 0)
            {

                if (DateTime.Compare(zquarterOneStart, zquarterOneEnd) < 0 && DateTime.Compare(zquarterTwoStart, zquarterTwoEnd) < 0 &&
                    DateTime.Compare(zquarterThreeStart, zquarterThreeEnd) < 0 && DateTime.Compare(zquarterFourStart, zquarterFourEnd) < 0 &&
                    DateTime.Compare(yearStartOne, yearEndOne) < 0)
                {

                    if (DateTime.Compare(zquarterOneEnd, zquarterTwoStart) < 0 && DateTime.Compare(zquarterTwoEnd, zquarterThreeStart) < 0 &&
                    DateTime.Compare(zquarterThreeEnd, zquarterFourStart) < 0 && (DateTime.Compare(zquarterFourEnd, yearEndOne) < 0 || DateTime.Compare(zquarterFourEnd, yearEndOne) == 0))
                    {
                        int y = 0;
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
        public bool IsTeacherorUnitLeader(string uid, string role)
        {
            //function for listing every user with registrar role 
            var appDbContext = new ApplicationDbContext();
            var userStore = new ApplicationUserStore(appDbContext);
            var userManager = new ApplicationUserManager(userStore);
            ApplicationUser user = new ApplicationUser();
            RegisterTeacherViewModel rvm = new RegisterTeacherViewModel();

            List<ApplicationUser> users = new List<ApplicationUser>();
            users = appDbContext.Users.ToList();
            if (role == "Teacher") {

                if (userManager.IsInRole(uid, "Teacher"))
                {
                    return true;
                }


            }
            else if (role == "UnitLeader")
            {

                if (userManager.IsInRole(uid, "UnitLeader"))
                {
                    return true;
                }

            }
            return false;
        }

        public AcademicYearViewModel listAcademicYear()
        {
           
            ApplicationDbContext db = new ApplicationDbContext();
            AcademicYearViewModel ayVM = new AcademicYearViewModel();
            ayVM.academicList = new List<AcademicYear>();
            ayVM.academicList = db.AcademicYear.ToList();
            return ayVM;
        }
    }
}
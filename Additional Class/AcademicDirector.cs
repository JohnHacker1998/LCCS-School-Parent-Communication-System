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
        public void registerTeacher(Teacher teacher)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            context.Teacher.Add(teacher);
            context.SaveChanges();
        }

        public void UpdateTeacher(Teacher teacher)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            Teacher teacherUP = new Teacher(1);
            teacherUP = context.Teacher.Find(teacher.teacherId);
            teacherUP.subject = teacher.subject;
            teacherUP.user.fullName = teacher.user.fullName;
            teacherUP.grade = teacher.grade;
            context.SaveChanges();
        }
        public void DeleteTeacher(string id)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            Teacher teacher = new Teacher();
            teacher = context.Teacher.Find(id);
            context.Teacher.Remove(teacher);
            context.SaveChanges();
        }
        public RegistrarManagementViewModel listRegistrar()
        {
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
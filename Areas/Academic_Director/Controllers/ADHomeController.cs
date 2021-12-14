
using LCCS_School_Parent_Communication_System.Additional_Class;
using LCCS_School_Parent_Communication_System.Identity;
using LCCS_School_Parent_Communication_System.Models;
using LCCS_School_Parent_Communication_System.viewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LCCS_School_Parent_Communication_System.Areas.Academic_Director.Controllers
{
    [Authorize(Roles = "AcademicDirector")]

    public class ADHomeController : Controller
    {
        // GET: Academic_Director/ADHome
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RegisterTeacher(string Id)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            RegisterTeacherViewModel registerTeacherViewModel = new RegisterTeacherViewModel();
            if (Id != null)
            {
                ViewBag.register = false;
                Teacher teacher = new Teacher();
                teacher = context.Teacher.Find(Id);
                registerTeacherViewModel.teacherList.Add(teacher);
            }
            return View(registerTeacherViewModel);
        }

        [HttpPost]
        public ActionResult RegisterTeacher(RegisterTeacherViewModel registerTeacherViewModel,string register,string search,string update)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            Collection collection = new Collection();
            RegistrarMethods registrarMethods = new RegistrarMethods();
            Teacher teacher = new Teacher();
            RegisterViewModel registerViewModel = new RegisterViewModel();

            ViewBag.register = true;
            ViewBag.search = false;

            if (register != null)
            {
                
                registerViewModel.fullName = registerTeacherViewModel.fullName;
                registerViewModel.email = registerTeacherViewModel.email;
                registerViewModel.username = collection.generateUserName();
                registerViewModel.password = collection.generatePassword();

                string id = collection.RegisterUser(registerViewModel, "Teacher");

                if (id != null)
                {
                    teacher.teacherId = id;
                    teacher.grade = registerTeacherViewModel.grade;
                    teacher.subject = registerTeacherViewModel.subject;

                    registrarMethods.registerTeacher(teacher);
                }
            }
            else if (search != null)
            {
                ViewBag.search = true;
                registerTeacherViewModel.teacherList = context.Teacher.Where(t => t.user.fullName.StartsWith(registerTeacherViewModel.fullName)).ToList();
            }
            else if (update != null)
            {
                foreach(var teacherobj in registerTeacherViewModel.teacherList)
                {
                    teacher.teacherId = teacherobj.teacherId;
                    teacher.subject = teacherobj.subject;
                    teacher.grade = teacherobj.grade;
                    teacher.user.fullName = teacherobj.user.fullName;
                    registrarMethods.UpdateTeacher(teacher);
                }
            }
            return View(registerTeacherViewModel);
        }

        //public ActionResult UpdateTeacher(string Id)
        //{
        //    //populate the new data
        //    //create update user function
        //    //call the function

        //    return RedirectToAction("RegisterTeacher", "ADHome", new { Areas = "Academic_Director", Id = Id });
        //}
    }
}
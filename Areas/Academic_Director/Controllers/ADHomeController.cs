
using LCCS_School_Parent_Communication_System.Additional_Class;
using LCCS_School_Parent_Communication_System.Identity;
using LCCS_School_Parent_Communication_System.Models;
using LCCS_School_Parent_Communication_System.viewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LCCS_School_Parent_Communication_System.viewModels;
using LCCS_School_Parent_Communication_System.Additional_Class;
using LCCS_School_Parent_Communication_System.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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

        public ActionResult RegisterTeacher()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            RegisterTeacherViewModel registerTeacherViewModel = new RegisterTeacherViewModel();
            //if (Id != null)
            //{
            //    ViewBag.register = false;
            //    Teacher teacher = new Teacher();
            //    teacher = context.Teacher.Find(Id);
            //    registerTeacherViewModel.teacherList.Add(teacher);
            //}
            return View(registerTeacherViewModel);
        }

        [HttpPost]
        public ActionResult RegisterTeacher(RegisterTeacherViewModel registerTeacherViewModel,string register,string search,string update,string edit,string delete,string Id)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            Collection collection = new Collection();
            AcademicDirector academicDirector = new AcademicDirector();
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

                    academicDirector.registerTeacher(teacher);
                }
            }
            else if (search != null)
            {
                ViewBag.search = true;
                registerTeacherViewModel.teacherList = context.Teacher.Where(t => t.user.fullName.StartsWith(registerTeacherViewModel.fullName)).ToList();
            }
            else if (update != null)
            {

                teacher.user.fullName = registerTeacherViewModel.fullName;
                teacher.grade = registerTeacherViewModel.grade;
                teacher.subject = registerTeacherViewModel.subject;

                foreach (var teacherobj in registerTeacherViewModel.teacherList)
                {
                    teacher.teacherId = teacherobj.teacherId;
                    
                }

                academicDirector.UpdateTeacher(teacher);
            }
            else if (edit !=null)
            {
                ViewBag.disableEmail = "disabled";
                teacher = context.Teacher.Find(Id);
                registerTeacherViewModel.fullName=teacher.user.fullName;
                registerTeacherViewModel.subject = teacher.subject;
                registerTeacherViewModel.grade = teacher.grade;
                registerTeacherViewModel.teacherList.Add(teacher);

            }
            else if (delete != null)
            {
                collection.DeleteUser(Id);
                academicDirector.DeleteTeacher(Id);
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
       
        public ActionResult manageRegistrar()
        {
            var appDbContext = new ApplicationDbContext();
            
            var userStore = new ApplicationUserStore(appDbContext);
            var userManager = new ApplicationUserManager(userStore);
            ApplicationUser user = new ApplicationUser();
            RegistrarManagementViewModel rvm = new RegistrarManagementViewModel();

            List<ApplicationUser> users = new List<ApplicationUser>();
            users = appDbContext.Users.ToList();
            
            foreach(var k in users)
            {
                if (userManager.IsInRole(k.Id, "Registrar"))
                {
                    rvm.registrarList.Add(new ApplicationUser { 
                        Id=k.Id,
                        fullName= k.fullName,
                        UserName=k.UserName,
                        Email= k.Email 
                        
                    });
                }
                           
            }
         
            int x = 0; 
            return View(rvm);
        }
    
        [HttpPost]
        public ActionResult manageRegistrar(RegistrarManagementViewModel rmv){
            RegisterViewModel rv = new RegisterViewModel();
            Collection c = new Collection();
            rv.username = c.generateUserName();
            rv.password = c.generatePassword();
            rv.email = rmv.email;
            rv.fullName = rmv.fullName;
            c.RegisterUser(rv, "Registrar");
            string messageBody = "Registrar Account Username:" + rv.username + "Password=" + rv.password;
            //   c.sendMail(rv.username, messageBody);
            return View();
        }
        public async Task<ActionResult> deleteRegistrar(string theID)
        {
           // Collection collection = new Collection();
            int x = 0;
         //   collection.DeleteUser(theID);
            var appDbContext = new ApplicationDbContext();
            var userStore = new ApplicationUserStore(appDbContext);
            var userManager = new ApplicationUserManager(userStore);

            var user = await userManager.FindByIdAsync(theID);
            var result = await userManager.DeleteAsync(user);
            
            return RedirectToAction("Index");
        }

    }
}

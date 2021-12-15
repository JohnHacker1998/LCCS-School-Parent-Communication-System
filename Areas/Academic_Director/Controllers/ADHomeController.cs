
using LCCS_School_Parent_Communication_System.Additional_Class;
using LCCS_School_Parent_Communication_System.Identity;
using LCCS_School_Parent_Communication_System.Models;
using LCCS_School_Parent_Communication_System.viewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;


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
            //ApplicationDbContext context = new ApplicationDbContext();
            //RegisterTeacherViewModel registerTeacherViewModel = new RegisterTeacherViewModel();
            //if (Id != null)
            //{
            //    ViewBag.register = false;
            //    Teacher teacher = new Teacher();
            //    teacher = context.Teacher.Find(Id);
            //    registerTeacherViewModel.teacherList.Add(teacher);
            //}
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> RegisterTeacher(RegisterTeacherViewModel registerTeacherViewModel,string register,string search,string update,string edit,string delete,string id)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            Collection collection = new Collection();
            AcademicDirector academicDirector = new AcademicDirector();
            Teacher teacher = new Teacher();
            ApplicationUser user = new ApplicationUser();
            RegisterViewModel registerViewModel = new RegisterViewModel();

            ViewBag.register = true;
            ViewBag.search = false;

            if (register != null)
            {
                
                registerViewModel.fullName = registerTeacherViewModel.fullName;
                registerViewModel.email = registerTeacherViewModel.email;
                registerViewModel.username = collection.generateUserName();
                registerViewModel.password = collection.generatePassword();

                string Id = collection.RegisterUser(registerViewModel, "Teacher");

                if (Id != null)
                {
                    teacher.teacherId = Id;
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
                Teacher teacherUp = new Teacher(1);
                teacherUp.grade = registerTeacherViewModel.grade;
                teacherUp.user.fullName = registerTeacherViewModel.fullName;
                teacherUp.subject = registerTeacherViewModel.subject;
                var teacherList= context.Teacher.Where(t=> t.user.Email==registerTeacherViewModel.email).ToList();
                foreach (var getId in teacherList)
                {
                    teacherUp.teacherId = getId.teacherId;
                }
                academicDirector.UpdateTeacher(teacherUp);
            }
            else if (edit !=null)
            {
                
                registerTeacherViewModel.teacherList = new List<Teacher>();
                ViewBag.disableEmail = "disabled";
                teacher = context.Teacher.Find(id);
                registerTeacherViewModel.fullName=teacher.user.fullName;
                registerTeacherViewModel.subject = teacher.subject;
                registerTeacherViewModel.grade = teacher.grade;
                registerTeacherViewModel.email = teacher.user.Email;
                registerTeacherViewModel.teacherList.Add(teacher);
                

            }
            else if (delete != null)
            {
                academicDirector.DeleteTeacher(id);
                string status=await collection.DeleteUser(id);
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
            RegistrarManagementViewModel rmvm = new RegistrarManagementViewModel();
            AcademicDirector ad = new AcademicDirector();
            
           rmvm=ad.listRegistrar();
           
            return View(rmvm);
        }
    
        [HttpPost]
        public async Task<ActionResult> manageRegistrar(RegistrarManagementViewModel rmv,string register,string delete,string theID){
            RegisterViewModel rv = new RegisterViewModel();
            Collection c = new Collection();
            if (register != null)
            {
                rv.username = c.generateUserName();
                rv.password = c.generatePassword();
                rv.email = rmv.email;
                rv.fullName = rmv.fullName;
                c.RegisterUser(rv, "Registrar");
                string messageBody = "Registrar Account Username:" + rv.username + "Password=" + rv.password;
                //   c.sendMail(rv.username, messageBody);
            }
            else if (delete != null)
            {
                var appDbContext = new ApplicationDbContext();
                var userStore = new ApplicationUserStore(appDbContext);
                var userManager = new ApplicationUserManager(userStore);

                var user = await userManager.FindByIdAsync(theID);
                var result = await userManager.DeleteAsync(user);
            }

            RegistrarManagementViewModel rmvm = new RegistrarManagementViewModel();
            AcademicDirector ad = new AcademicDirector();

            rmvm = ad.listRegistrar();

            return View(rmvm);
        }
       /* public async Task<ActionResult> deleteRegistrar(string theID)
        {
           // Collection collection = new Collection();
            
         //   collection.DeleteUser(theID);
            var appDbContext = new ApplicationDbContext();
            var userStore = new ApplicationUserStore(appDbContext);
            var userManager = new ApplicationUserManager(userStore);

            var user = await userManager.FindByIdAsync(theID);
            var result = await userManager.DeleteAsync(user);

           
            return View("manageRegistrar");
        }*/

    }
}

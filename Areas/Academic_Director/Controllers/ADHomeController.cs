
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
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> RegisterTeacher(RegisterTeacherViewModel registerTeacherViewModel,string register,string search,string update,string edit,string delete,string id)
        {
            //check if their are no errors arise from user input
            if (ModelState.IsValid)
            {
                //basic objects
                ApplicationDbContext context = new ApplicationDbContext();
                Collection collection = new Collection();
                AcademicDirector academicDirector = new AcademicDirector();
                Teacher teacher = new Teacher();
                ApplicationUser user = new ApplicationUser();
                RegisterViewModel registerViewModel = new RegisterViewModel();

                ViewBag.search = false;

                if (register != null)
                {

                    //populate RegisterViewModel with the inserted data for registration
                    registerViewModel.fullName = registerTeacherViewModel.fullName;
                    registerViewModel.email = registerTeacherViewModel.email;
                    registerViewModel.username = collection.generateUserName();
                    registerViewModel.password = collection.generatePassword();

                    //create teacher user account using the provided information
                    string Id = collection.RegisterUser(registerViewModel, "Teacher");

                    if (Id != null)
                    {
                        //record other teacher informations
                        teacher.teacherId = Id;
                        teacher.grade = registerTeacherViewModel.grade;
                        teacher.subject = registerTeacherViewModel.subject;

                        academicDirector.registerTeacher(teacher);

                        //email code goes here------------------------------

                        ViewBag.registerStatus = "Registration Completed Successfully";
                    }
                    else
                    {
                        ViewBag.registerStatus = "Registration Failed";
                    }
                }
                else if (search != null)
                {
                    //search teacher using teacher name
                    ViewBag.search = true;
                    registerTeacherViewModel.teacherList = context.Teacher.Where(t => t.user.fullName.StartsWith(registerTeacherViewModel.fullName)).ToList();
                }
                else if (update != null)
                {
                    //update teacher record 
                    //assign the new data to teacher object
                    Teacher teacherUp = new Teacher(1);
                    teacherUp.grade = registerTeacherViewModel.grade;
                    teacherUp.user.fullName = registerTeacherViewModel.fullName;
                    teacherUp.subject = registerTeacherViewModel.subject;
                    var teacherList = context.Teacher.Where(t => t.user.Email == registerTeacherViewModel.email).ToList();
                    foreach (var getId in teacherList)
                    {
                        teacherUp.teacherId = getId.teacherId;
                    }

                    academicDirector.UpdateTeacher(teacherUp);

                    ViewBag.updateStatus = "Update Completed Successfully";
                }
                else if (edit != null)
                {
                    //populate the selected teacher data in to the update form
                    registerTeacherViewModel.teacherList = new List<Teacher>();
                    teacher = context.Teacher.Find(id);
                    registerTeacherViewModel.fullName = teacher.user.fullName;
                    registerTeacherViewModel.subject = teacher.subject;
                    registerTeacherViewModel.grade = teacher.grade;
                    registerTeacherViewModel.email = teacher.user.Email;
                    registerTeacherViewModel.teacherList.Add(teacher);

                    ViewBag.disableEmail = "disabled";
                }
                else if (delete != null)
                {
                    //delete the selected user using teacher id
                    academicDirector.DeleteTeacher(id);
                    string status = await collection.DeleteUser(id);

                    if (status == "successful")
                    {
                        ViewBag.deleteStatus = "Deletion Completed Successfully";
                    }
                    else
                    {
                        ViewBag.deleteStatus = "Deletion Failed";
                    }

                }
            }
            return View(registerTeacherViewModel);
        }
       
        public ActionResult manageRegistrar()

        {
            //displaying list of users with registrar role
            RegistrarManagementViewModel rmvm = new RegistrarManagementViewModel();
            AcademicDirector ad = new AcademicDirector();
            
           rmvm=ad.listRegistrar();
           
            return View(rmvm);
        }
    
        [HttpPost]
        public async Task<ActionResult> manageRegistrar(RegistrarManagementViewModel rmv,string register,string delete,string theID){
            RegisterViewModel rv = new RegisterViewModel();
            Collection c = new Collection();
            RegistrarManagementViewModel rmvm = new RegistrarManagementViewModel();
            AcademicDirector ad = new AcademicDirector();
            //checking the validity of the inputs
            if (ModelState.IsValid)
            {
                //checking if register button is clicked
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

            }
            //checking if delete button is clicked
            if (delete != null)
            {
                string status = await c.DeleteUser(theID);
            }
            //refreshing the list of users with registrar role.
            rmvm = ad.listRegistrar();

            return View(rmvm);
        }
       

    }
}

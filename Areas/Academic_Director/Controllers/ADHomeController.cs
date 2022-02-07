
using LCCS_School_Parent_Communication_System.Additional_Class;
using LCCS_School_Parent_Communication_System.Identity;
using LCCS_School_Parent_Communication_System.Models;
using LCCS_School_Parent_Communication_System.viewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Helpers;
using System.Text.RegularExpressions;

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

       

        public ActionResult Register()
        {
            return PartialView("Register");
        }
        
        [HttpPost]
        public ActionResult Register(RegisterTeacherModal registerTeacher)
        {
            ViewBag.complete = null;
            ViewBag.error = null;

            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            var userStore = new ApplicationUserStore(context);
            var userManager = new ApplicationUserManager(userStore);
            Collection collection = new Collection();
            RegisterViewModel registerViewModel = new RegisterViewModel();
            Models.Teacher teacher = new Models.Teacher();
            AcademicDirector academicDirector = new AcademicDirector();

            registerTeacher.fullName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(registerTeacher.fullName.ToLower());

            //check for a duplicate record
            if (collection.checkUserExistence(registerTeacher.email, registerTeacher.fullName))
            {
                //populate RegisterViewModel with the inserted data for registration
                registerViewModel.fullName = registerTeacher.fullName; 
                registerViewModel.email = registerTeacher.email;
                do
                {
                    //check if the username is unique if not regenerate
                    registerViewModel.username = collection.generateUserName();
                }
                while (userManager.FindByName(registerViewModel.username) != null);

                registerViewModel.password = collection.generatePassword();

                //create teacher user account using the provided information
                string Id = collection.RegisterUser(registerViewModel, "Teacher");

                //check if the user registration is completed successfully and record to teacher table  
                if (Id != null)
                {
                    //record other teacher informations
                    teacher.teacherId = Id;
                    teacher.grade = registerTeacher.grade;
                    teacher.subject = registerTeacher.subject;

                    Boolean sucess = academicDirector.registerTeacher(teacher);

                    //check if the operation completed successfully or not
                    if (sucess)
                    {
                        //send user credential through email to the new user
                        Boolean mail = collection.sendMail(registerViewModel.email, registerViewModel.username, registerViewModel.password);

                        if (mail)
                        {
                            //sucessful message
                            ViewBag.complete = "Registration Completed Successfully";
                        }
                        else
                        {
                            //delete the created user record
                            var revert = context.Users.Find(Id);
                            context.Users.Remove(revert);
                            int result = 0;
                            do
                            {
                                result = context.SaveChanges();
                            }
                            while (result == 0);

                            ViewBag.error = "Registration Failed!! Email Address doesn't exist";
                        }

                    }
                    else
                    {
                        //delete the created user record
                        var revert = context.Users.Find(Id);
                        context.Users.Remove(revert);
                        int result = 0;
                        do
                        {
                            result = context.SaveChanges();
                        }
                        while (result == 0);

                        ViewBag.error = "Registration Failed!!";
                    }

                }
                else
                {
                    //faliure message due to identity register failure
                    ViewBag.error = "Registration Failed!!";
                }
            }
            else
            {
                //failure message due to duplicate user
                ViewBag.error = "Registration Failed!! Record Exists with the Same Email Address or Full Name";
            }

            return PartialView("Register");
        }

        public ActionResult EditTeacher(string id)
        {
            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            var userStore = new ApplicationUserStore(context);
            var userManager = new ApplicationUserManager(userStore);
            UpdateTeacherModal updateTeacher = new UpdateTeacherModal();
            Models.Teacher teacher = new Models.Teacher();

            //check teacher role before editing
            if (!(userManager.IsInRole(id, "UnitLeader") || userManager.IsInRole(id, "HomeRoom")))
            {
                //populate the selected teacher data in to the update form
                teacher = context.Teacher.Find(id);
                updateTeacher.Id = id;
                updateTeacher.fullName = teacher.user.fullName;
                updateTeacher.subject = teacher.subject;
                updateTeacher.grade = teacher.grade;

            }
            else
            {
                //error message for additional role
                ViewBag.geterror = "Unable To Edit. Teacher has Another Role Associated";
            }
            return PartialView("EditTeacher",updateTeacher);
        }

        [HttpPost]
        public ActionResult EditTeacher(UpdateTeacherModal updateTeacher)
        {
            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            Models.Teacher teacherUp = new Models.Teacher(1);
            AcademicDirector academicDirector = new AcademicDirector();

            var checkFullName = context.Users.Where(u => u.fullName == updateTeacher.fullName && u.Id != updateTeacher.Id).FirstOrDefault();

            //check if no duplicate name exist
            if (checkFullName == null)
            {
                teacherUp.teacherId = updateTeacher.Id;
                teacherUp.grade = updateTeacher.grade;
                teacherUp.user.fullName = updateTeacher.fullName;
                teacherUp.subject = updateTeacher.subject;

                //update teacher record 
                Boolean result= academicDirector.UpdateTeacher(teacherUp);

                if (result)
                {
                    //update successful message
                    ViewBag.complete = "Update Completed Successfully";
                }
                else
                {
                    ViewBag.error = "Update Failed!!";
                }
                
            }
            else
            {
                //error message for duplicate Full Name
                ViewBag.error = "Full Name Already Taken By Another Account";
            }
            return PartialView("EditTeacher",updateTeacher);
        }


        public ActionResult DeleteTeacher(string id)
        {
            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            var userStore = new ApplicationUserStore(context);
            var userManager = new ApplicationUserManager(userStore);
            DeleteTeacherModal deleteTeacher = new DeleteTeacherModal();

            deleteTeacher.Id = id;
            //check role
            if (!(userManager.IsInRole(id, "UnitLeader") || userManager.IsInRole(id, "HomeRoom")))
            {
                ViewBag.message = "Are You Sure Do You Want Delete This Teacher?";
            }
            else
            {
                //error message due to additional role
                ViewBag.error = "Unable To Delete. Teacher has Another Role Associated";

            }
            return PartialView("DeleteTeacher",deleteTeacher);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteTeacher(DeleteTeacherModal deleteTeacher)
        {
            AcademicDirector academicDirector = new AcademicDirector();
            Collection collection = new Collection();

            Boolean delete= academicDirector.DeleteTeacher(deleteTeacher.Id);
            if (delete)
            {
                string status = await collection.DeleteUser(deleteTeacher.Id);

                if (status == "successful")
                {
                    //sucess message
                    ViewBag.complete = "Deletion Completed Successfully";                 
                }
                else
                {
                    //failure message
                    ViewBag.posterror = "Deletion Failed";
                }
            }
            else
            {
                ViewBag.posterror = "Deletion Failed";
            }
            
            return PartialView("DeleteTeacher");
        }

        public ActionResult RegisterTeacher()
        {

            //handel viewbag
            //ViewBag.search = false;
            //ViewBag.upHidden = "hidden";
            //ViewBag.disableEmail = false;

            ApplicationDbContext context = new ApplicationDbContext();
            RegisterTeacherViewModel registerTeacherViewModel = new RegisterTeacherViewModel();
            registerTeacherViewModel.teacherList = context.Teacher.ToList();

            return View(registerTeacherViewModel);
        }

        //[HttpPost]
        //public async Task<ActionResult> RegisterTeacher(RegisterTeacherViewModel registerTeacherViewModel,string register,string search,string update,string edit,string delete,string id)
        //{
        //    //viewbag attributes for UI rendering 
        //    ViewBag.search = false;
        //    ViewBag.upHidden = "hidden";
        //    ViewBag.disableEmail = false;
        //    ApplicationDbContext context = new ApplicationDbContext();
            
        //    registerTeacherViewModel.teacherList = context.Teacher.ToList();
        //    //check if their are no errors arise from user input
        //    if (ModelState.IsValid || (search!=null && ModelState.IsValidField("fullName")) || edit!=null || delete!=null)
        //    {
        //        //basic objects
        //        //ApplicationDbContext context = new ApplicationDbContext();
        //        var userStore = new ApplicationUserStore(context);
        //        var userManager = new ApplicationUserManager(userStore);
        //        Collection collection = new Collection();
        //        AcademicDirector academicDirector = new AcademicDirector();
        //        Models.Teacher teacher = new Models.Teacher();
        //        ApplicationUser user = new ApplicationUser();
        //        RegisterViewModel registerViewModel = new RegisterViewModel();

        //        //check if register button is clicked
        //        if (register != null)
        //        {
        //            //check for a duplicate record
        //            if (collection.checkUserExistence(registerTeacherViewModel.email, registerTeacherViewModel.fullName))
        //            {
        //                //populate RegisterViewModel with the inserted data for registration
        //                registerViewModel.fullName = registerTeacherViewModel.fullName;
        //                registerViewModel.email = registerTeacherViewModel.email;
        //                do
        //                {
        //                    //check if the username is unique if not regenerate
        //                    registerViewModel.username = collection.generateUserName();
        //                }
        //                while (userManager.FindByName(registerViewModel.username) != null);

        //                registerViewModel.password = collection.generatePassword();

        //                //create teacher user account using the provided information
        //                string Id = collection.RegisterUser(registerViewModel, "Teacher");

        //                //check if the user registration is completed successfully and record to teacher table  
        //                if (Id != null)
        //                {
        //                    //record other teacher informations
        //                    teacher.teacherId = Id;
        //                    teacher.grade = registerTeacherViewModel.grade;
        //                    teacher.subject = registerTeacherViewModel.subject;

        //                    academicDirector.registerTeacher(teacher);

        //                    //send user credential through email to the new user
        //                    collection.sendMail(registerViewModel.email, registerViewModel.username, registerViewModel.password);

        //                    //sucessful message
        //                    ViewBag.registerStatus = "Registration Completed Successfully";
        //                }
        //                else
        //                {
        //                    //faliure message due to identity register failure
        //                    ViewBag.registerStatus = "Registration Failed";
        //                }
        //            }
        //            else
        //            {
        //                //failure message due to duplicate user
        //                ViewBag.duplicate = "Teacher Record Exists with the Same Email Address or Full Name";
        //            }
        //        }
        //        //check if search button is clicked
        //        else if (search != null)
        //        {
        //            //remove unwanted error messages
        //            ModelState.Remove("email");
        //            ModelState.Remove("grade");
        //            ModelState.Remove("subject");

        //            //viewbag element for UI 
        //            ViewBag.search = true;

        //            //search teacher using teacher name
        //            registerTeacherViewModel.teacherList = context.Teacher.Where(t => t.user.fullName.StartsWith(registerTeacherViewModel.fullName)).ToList();

        //            //check if record exist or not
        //            if (registerTeacherViewModel.teacherList.Count == 0)
        //            {
        //                ViewBag.search = false;
        //                ViewBag.searchFound = "Record Not Found";
        //            }
        //        }
        //        //check if update button is clicked
        //        else if (update != null)
        //        {
        //            //teacher object
        //            Models.Teacher teacherUp = new Models.Teacher(1);

        //            //assign the new data to teacherUp object
        //            var getId = context.Teacher.Where(t => t.user.Email == registerTeacherViewModel.email).FirstOrDefault();
        //            teacherUp.teacherId = getId.teacherId;
                    
        //            var checkFullName = context.Users.Where(u => u.fullName == registerTeacherViewModel.fullName && u.Id != teacherUp.teacherId).FirstOrDefault();
                    
        //            //check if no duplicate name exist
        //            if (checkFullName == null)
        //            {
        //                teacherUp.grade = registerTeacherViewModel.grade;
        //                teacherUp.user.fullName = registerTeacherViewModel.fullName;
        //                teacherUp.subject = registerTeacherViewModel.subject;

        //                //update teacher record 
        //                academicDirector.UpdateTeacher(teacherUp);

        //                //update successful message
        //                ViewBag.updateStatus = "Update Completed Successfully";
        //                //ViewBag.upHidden = "hidden";
        //                ViewBag.disableEmail = false;
        //            }
        //            else
        //            {
        //                //error message for duplicate Full Name
        //                ViewBag.fullName = "Full Name Already Taken By Another Account";
        //            }
        //        }
        //        //check if edit button is clicked
        //        else if (edit != null)
        //        {
        //            //remove unwanted error messages
        //            ModelState.Clear();

        //            //check teacher role before editing
        //            if (!(userManager.IsInRole(id, "UnitLeader") || userManager.IsInRole(id, "HoomRoom")))
        //            {
        //                //populate the selected teacher data in to the update form
        //                registerTeacherViewModel.teacherList = new List<Models.Teacher>();
        //                teacher = context.Teacher.Find(id);
        //                registerTeacherViewModel.fullName = teacher.user.fullName;
        //                registerTeacherViewModel.subject = teacher.subject;
        //                registerTeacherViewModel.grade = teacher.grade;
        //                registerTeacherViewModel.email = teacher.user.Email;
        //                registerTeacherViewModel.teacherList.Add(teacher);

        //                //viewbag elements
        //                ViewBag.disableEmail = true;
        //                ViewBag.upHidden = " ";
        //            }
        //            else
        //            {
        //                //error message for additional role
        //                ViewBag.role = "Unable To Edit Because Teacher has Another Role Associated";
        //                ViewBag.upHidden = "hidden";
        //            }
                    
        //        }
        //        //check if delete button is clicked
        //        else if (delete != null)
        //        {
        //            //viewbag elements
        //            ViewBag.search = false;
        //            ViewBag.upHidden = "hidden";

        //            //check role
        //            if (!(userManager.IsInRole(id, "UnitLeader") && userManager.IsInRole(id, "HoomRoom")))
        //            {
        //                //delete the selected user using teacher id
        //                academicDirector.DeleteTeacher(id);
        //                string status = await collection.DeleteUser(id);

        //                if (status == "successful")
        //                {
        //                    //sucess message
        //                    ViewBag.deleteStatus = "Deletion Completed Successfully";
        //                }
        //                else
        //                {
        //                    //failure message
        //                    ViewBag.deleteStatus = "Deletion Failed";
        //                }
        //            }
        //            else
        //            {
        //                //error message due to additional role
        //                ViewBag.role = "Unable To Delete Because Teacher has Another Role Associated";
                        
        //            }
        //        }
        //    }
        //    return View(registerTeacherViewModel);
        //}
       public ActionResult registerRegistrar()
        {
            return PartialView("registerRegistrar");
        }
        [HttpPost]
        public ActionResult registerRegistrar(RegistrarManagementViewModel rmv)
        {
            RegisterViewModel rv = new RegisterViewModel();
            Collection c = new Collection();
            RegistrarManagementViewModel rmvm = new RegistrarManagementViewModel();
            AcademicDirector ad = new AcademicDirector();
            ViewBag.Message = " ";
            ViewBag.addedSuccessfully = " ";
            //checking the validity of the inputs
            if (ModelState.IsValid)
            {
                //checking if register button is clicked
              
                    if (c.checkUserExistence(rmv.email, rmv.fullName))
                    {
                        rv.username = c.generateUserName();
                        rv.password = c.generatePassword();
                        rv.email = rmv.email;
                        rv.fullName = rmv.fullName;
                        c.RegisterUser(rv, "Registrar");
                        
                        c.sendMail(rv.email, rv.username, rv.password);
                        ViewBag.addedSuccessfully = "User Added Successfully.";
                    }
                    else
                    {
                        ViewBag.Message = "User exists";
                    }
                

            }
          
            return PartialView("registerRegistrar");
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
            ViewBag.Message = " ";
            //checking the validity of the inputs
            if (ModelState.IsValid)
            {
                //checking if register button is clicked
                if (register != null)
                {
                  if(c.checkUserExistence(rmv.email, rmv.fullName)){ 
                    rv.username = c.generateUserName();
                    rv.password = c.generatePassword();
                    rv.email = rmv.email;
                    rv.fullName = rmv.fullName;
                    c.RegisterUser(rv, "Registrar");
                   // string messageBody = "Registrar Account Username:" + rv.username + "Password=" + rv.password;
                    c.sendMail(rv.email,rv.username, rv.password);
                    }
                    else
                    {
                        ViewBag.Message = "User exists";
                    }
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
        public ActionResult registerAcademicYear()
        {
            return PartialView("registerAcademicYear");
        }

        [HttpPost]
         public ActionResult registerAcademicYear(AcademicYearViewModel ayvm)
        {
            ViewBag.addedSuccessfully = " ";
            ViewBag.Message = " ";
            ViewBag.durationMessage = " ";

            AcademicYear ay = new AcademicYear();
            ApplicationDbContext db = new ApplicationDbContext();
            AcademicDirector ad = new AcademicDirector();
            AcademicYear ayear = new AcademicYear();
            if (ModelState.IsValid)
            {
                ay.academicYearName = Convert.ToDateTime(ayvm.yearStart).ToString("MMMM") + Convert.ToDateTime(ayvm.yearStart).Year.ToString();
                ay.durationStart = Convert.ToDateTime(ayvm.yearStart).Date;
                ay.durationEnd = Convert.ToDateTime(ayvm.yearEnd).Date;
                ay.quarterOneStart = Convert.ToDateTime(ayvm.quarterOneStart).Date;
                ay.quarterOneEnd = Convert.ToDateTime(ayvm.quarterOneEnd).Date;
                ay.quarterTwoStart = Convert.ToDateTime(ayvm.quarterTwoStart).Date;
                ay.quarterTwoEnd = Convert.ToDateTime(ayvm.quarterTwoEnd).Date;
                ay.quarterThreeStart = Convert.ToDateTime(ayvm.quarterThreeStart).Date;
                ay.quarterThreeEnd = Convert.ToDateTime(ayvm.quarterThreeEnd).Date;
                ay.quarterFourStart = Convert.ToDateTime(ayvm.quarterFourStart).Date;
                ay.quarterFourEnd = Convert.ToDateTime(ayvm.quarterFourEnd).Date;

                //concatenating year start month name and year to create the academic year name.

                //checking if the academic year name exists on the academic year table, and if so,not enabling user to add other duplicate data.
                ayear = db.AcademicYear.Where(a => a.academicYearName == ay.academicYearName).FirstOrDefault();
                if (ayear == null)
                {
                    //checking if the recieved academic year information fulfills the necessary criterias using the validateDuration() method.
                    if (ad.validateDuration(ay))
                    {
                        //ifso enabling user to register the information to the academic year table.

                        db.AcademicYear.Add(ay);
                        db.SaveChanges();
                        ViewBag.addedSuccessfully = "AcademicYear added Successfully.";


                    }
                    else
                    {
                        ViewBag.durationMessage = "Duration is invalid";
                    }
                }
                else
                {
                    ViewBag.Message = "Academic Year Exists";
                }
            }

            return PartialView("registerAcademicYear");
        }
        public ActionResult updateAcademicYear(string id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            AcademicYearViewModel academicYearViewModel = new AcademicYearViewModel();
            ViewBag.disableYearStart = false;
            ViewBag.disableYearEnd = false;
            ViewBag.disableQuarterOneStart = false;
            ViewBag.disableQuarterOneEnd = false;
            ViewBag.disableQuarterTwoStart = false;
            ViewBag.disableQuarterTwoEnd = false;
            ViewBag.disableQuarterThreeStart = false;
            ViewBag.disableQuarterThreeEnd = false;
            ViewBag.disableQuarterFourStart = false;
            ViewBag.disableQuarterFourEnd = false;

            //searching the academic year using the academic year name to find the right academic year to populate the form.
            var academicYear = db.AcademicYear.Where(a => a.academicYearName == id).FirstOrDefault();


            academicYearViewModel.yearStart = academicYear.durationStart.ToShortDateString();
            ViewBag.disableYearStart = true;
            academicYearViewModel.yearEnd = academicYear.durationEnd.ToShortDateString();
            if (DateTime.Compare(DateTime.Now, academicYear.durationEnd) > 0)
            {
                ViewBag.disableYearEnd = true;
            }
            academicYearViewModel.quarterOneStart = academicYear.quarterOneStart.ToShortDateString();
            if (DateTime.Compare(DateTime.Now, academicYear.quarterOneStart) > 0)
            {
                ViewBag.disableQuarterOneStart = true;
            }
            academicYearViewModel.quarterOneEnd = academicYear.quarterOneEnd.ToShortDateString();
            if (DateTime.Compare(DateTime.Now, academicYear.quarterOneEnd) > 0)
            {
                ViewBag.disableQuarterOneEnd = true;
            }
            academicYearViewModel.quarterTwoStart = academicYear.quarterTwoStart.ToShortDateString();
            if (DateTime.Compare(DateTime.Now, academicYear.quarterTwoStart) > 0)
            {
                ViewBag.disableQuarterTwoStart = true;
            }
            academicYearViewModel.quarterTwoEnd = academicYear.quarterTwoEnd.ToShortDateString();
            if (DateTime.Compare(DateTime.Now, academicYear.quarterTwoEnd) > 0)
            {
                ViewBag.disableQuarterTwoEnd = true;
            }
            academicYearViewModel.quarterThreeStart = academicYear.quarterThreeStart.ToShortDateString();
            if (DateTime.Compare(DateTime.Now, academicYear.quarterThreeStart) > 0)
            {
                ViewBag.disableQuarterTwoEnd = true;
            }
            academicYearViewModel.quarterThreeEnd = academicYear.quarterThreeEnd.ToShortDateString();
            if (DateTime.Compare(DateTime.Now, academicYear.quarterThreeEnd) > 0)
            {
                ViewBag.disableQuarterThreeEnd = true;
            }
            academicYearViewModel.quarterFourStart = academicYear.quarterFourStart.ToShortDateString();
            if (DateTime.Compare(DateTime.Now, academicYear.quarterFourStart) > 0)
            {
                ViewBag.disableQuarterFourStart = true;
            }
            academicYearViewModel.quarterFourEnd = academicYear.quarterFourEnd.ToShortDateString();
            if (DateTime.Compare(DateTime.Now, academicYear.quarterFourEnd) > 0)
            {
                ViewBag.disableQuarterFourEnd = true;
            }

            ViewBag.disableUpdate = false;
            return PartialView(academicYearViewModel);
        }
        [HttpPost]
        public ActionResult updateAcademicYear(AcademicYearViewModel ayvm)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            AcademicDirector ad = new AcademicDirector();
            AcademicYear ay = new AcademicYear();
            if (ModelState.IsValid) {
                ay.academicYearName = Convert.ToDateTime(ayvm.yearStart).ToString("MMMM") + Convert.ToDateTime(ayvm.yearStart).Year.ToString();
                ay.durationStart = Convert.ToDateTime(ayvm.yearStart).Date;
                ay.durationEnd = Convert.ToDateTime(ayvm.yearEnd).Date;
                ay.quarterOneStart = Convert.ToDateTime(ayvm.quarterOneStart).Date;
                ay.quarterOneEnd = Convert.ToDateTime(ayvm.quarterOneEnd).Date;
                ay.quarterTwoStart = Convert.ToDateTime(ayvm.quarterTwoStart).Date;
                ay.quarterTwoEnd = Convert.ToDateTime(ayvm.quarterTwoEnd).Date;
                ay.quarterThreeStart = Convert.ToDateTime(ayvm.quarterThreeStart).Date;
                ay.quarterThreeEnd = Convert.ToDateTime(ayvm.quarterThreeEnd).Date;
                ay.quarterFourStart = Convert.ToDateTime(ayvm.quarterFourStart).Date;
                ay.quarterFourEnd = Convert.ToDateTime(ayvm.quarterFourEnd).Date;

                //if the modification of the academic year fulfills the necessary conditions stated in validateDuration(),allowing the update.
                if (ad.validateDuration(ay))
                {
                    //updating the academic year
                    AcademicYear ay1 = new AcademicYear();
                    ay1 = db.AcademicYear.Where(a => a.academicYearName == ay.academicYearName).FirstOrDefault();
                    if (ay1 != null)
                    {

                        db.SaveChanges();
                        ViewBag.SuccessMessage = "Academic Year updated successfully";

                    }
                }
                else
                {
                    ViewBag.durationMessage = "Duration is invalid";
                }
            }


        
            return PartialView("updateAcademicYear");
        }


        public ActionResult manageAcademicYear()
        {
            AcademicYearViewModel academicYearViewModel = new AcademicYearViewModel();
            AcademicDirector ad = new AcademicDirector();
            academicYearViewModel.academicList = new List<AcademicYear>();
            academicYearViewModel= ad.listAcademicYear();
            ViewBag.disableUpdate = true;

            return View(academicYearViewModel);
        }
        [HttpPost]
        public ActionResult manageAcademicYear(AcademicYearViewModel ayvm, string add,string update,string select,string acadYN)
        {
            AcademicYear ay = new AcademicYear();
            ApplicationDbContext db = new ApplicationDbContext();
            AcademicDirector ad = new AcademicDirector();
            ViewBag.disableUpdate = true;


            AcademicYearViewModel academicYearViewModel = new AcademicYearViewModel();
            academicYearViewModel.academicList = new List<AcademicYear>();
            academicYearViewModel = ad.listAcademicYear();
            ViewBag.Message = " ";
            ViewBag.durationMessage = " ";
         
            //setting up viewbags to enable and disable the datepicker class on the view
            ViewBag.disableYearStart = false;
            ViewBag.disableYearEnd = false;
            ViewBag.disableQuarterOneStart = false;
            ViewBag.disableQuarterOneEnd = false;
            ViewBag.disableQuarterTwoStart = false;
            ViewBag.disableQuarterTwoEnd = false;
            ViewBag.disableQuarterThreeStart = false;
            ViewBag.disableQuarterThreeEnd = false;
            ViewBag.disableQuarterFourStart = false;
            ViewBag.disableQuarterFourEnd = false;
            //on normal case,select and update don't need client-side validation due to update being dependent on select statement
            
            if (ModelState.IsValid || select!=null || update!=null)
            {
                //if the model is valid, strings reciieved from the view will be converted to datetime for further manipulation
               
                ay.academicYearName = Convert.ToDateTime(ayvm.yearStart).ToString("MMMM") + Convert.ToDateTime(ayvm.yearStart).Year.ToString();
               ay.durationStart = Convert.ToDateTime(ayvm.yearStart).Date;
               ay.durationEnd= Convert.ToDateTime(ayvm.yearEnd).Date;
                ay.quarterOneStart = Convert.ToDateTime(ayvm.quarterOneStart).Date;
                ay.quarterOneEnd = Convert.ToDateTime(ayvm.quarterOneEnd).Date;
                ay.quarterTwoStart = Convert.ToDateTime(ayvm.quarterTwoStart).Date;
               ay.quarterTwoEnd = Convert.ToDateTime(ayvm.quarterTwoEnd).Date;
                ay.quarterThreeStart = Convert.ToDateTime(ayvm.quarterThreeStart).Date;
                ay.quarterThreeEnd = Convert.ToDateTime(ayvm.quarterThreeEnd).Date;
                ay.quarterFourStart = Convert.ToDateTime(ayvm.quarterFourStart).Date;
                ay.quarterFourEnd = Convert.ToDateTime(ayvm.quarterFourEnd).Date;
                //if add is clicked
                if (add != null)
                {

                    AcademicYear ayear = new AcademicYear();
                    //concatenating year start month name and year to create the academic year name.
                  
                    //checking if the academic year name exists on the academic year table, and if so,not enabling user to add other duplicate data.
                    ayear = db.AcademicYear.Where(a => a.academicYearName == ay.academicYearName).FirstOrDefault();
                    if (ayear == null) { 
                        //checking if the recieved academic year information fulfills the necessary criterias using the validateDuration() method.
                    if (ad.validateDuration(ay))
                    {
                            //ifso enabling user to register the information to the academic year table.
                       
                        db.AcademicYear.Add(ay);
                        db.SaveChanges();
                                                    
                        }
                        else
                        {
                            ViewBag.durationMessage = "Duration is invalid";
                        }
                    }
                    else
                    {
                        ViewBag.Message="Academic Year Exists";
                    }

                }
                //if update is clicked

                else if (update != null)
                {
                   
                    ModelState.Clear();

                
               //if the modification of the academic year fulfills the necessary conditions stated in validateDuration(),allowing the update.
                    if (ad.validateDuration(ay))
                    {
                        //updating the academic year
                        AcademicYear ay1 = new AcademicYear();
                        ay1 = db.AcademicYear.Where(a => a.academicYearName == ay.academicYearName).FirstOrDefault();
                        if (ay1 != null) { 

                        db.SaveChanges();
                            ViewBag.disableUpdate = true;
                        }
                    }
                    else
                    {
                        ViewBag.durationMessage = "Duration is invalid";
                    }
                }
           //if select is clicked
                else if (select != null)
                {
                    ModelState.Clear();
                    //searching the academic year using the academic year name to find the right academic year to populate the form.
                    var academicYear = db.AcademicYear.Where(a => a.academicYearName == acadYN).FirstOrDefault();

                   
                        academicYearViewModel.yearStart = academicYear.durationStart.ToShortDateString();
                        ViewBag.disableYearStart = true;
                        academicYearViewModel.yearEnd = academicYear.durationEnd.ToShortDateString();
                         if (DateTime.Compare(DateTime.Now, academicYear.durationEnd) > 0)
                          {
                        ViewBag.disableYearEnd = true;
                           }
                        academicYearViewModel.quarterOneStart = academicYear.quarterOneStart.ToShortDateString();
                        if (DateTime.Compare(DateTime.Now, academicYear.quarterOneStart) > 0)
                         {
                        ViewBag.disableQuarterOneStart = true;
                         }
                        academicYearViewModel.quarterOneEnd = academicYear.quarterOneEnd.ToShortDateString();
                    if (DateTime.Compare(DateTime.Now, academicYear.quarterOneEnd) > 0)
                    {
                        ViewBag.disableQuarterOneEnd = true;
                    }
                    academicYearViewModel.quarterTwoStart = academicYear.quarterTwoStart.ToShortDateString();
                    if (DateTime.Compare(DateTime.Now, academicYear.quarterTwoStart) > 0)
                    {
                        ViewBag.disableQuarterTwoStart = true;
                    }
                    academicYearViewModel.quarterTwoEnd = academicYear.quarterTwoEnd.ToShortDateString();
                    if (DateTime.Compare(DateTime.Now, academicYear.quarterTwoEnd) > 0)
                    {
                        ViewBag.disableQuarterTwoEnd = true;
                    }
                    academicYearViewModel.quarterThreeStart = academicYear.quarterThreeStart.ToShortDateString();
                    if (DateTime.Compare(DateTime.Now, academicYear.quarterThreeStart) > 0)
                    {
                        ViewBag.disableQuarterTwoEnd = true;
                    }
                    academicYearViewModel.quarterThreeEnd = academicYear.quarterThreeEnd.ToShortDateString();
                    if (DateTime.Compare(DateTime.Now, academicYear.quarterThreeEnd) > 0)
                    {
                        ViewBag.disableQuarterThreeEnd = true;
                    }
                    academicYearViewModel.quarterFourStart = academicYear.quarterFourStart.ToShortDateString();
                    if (DateTime.Compare(DateTime.Now, academicYear.quarterFourStart) > 0)
                    {
                        ViewBag.disableQuarterFourStart = true;
                    }
                    academicYearViewModel.quarterFourEnd = academicYear.quarterFourEnd.ToShortDateString();
                    if (DateTime.Compare(DateTime.Now, academicYear.quarterFourEnd) > 0)
                    {
                        ViewBag.disableQuarterFourEnd = true;
                    }

                    ViewBag.disableUpdate = false;
                }
            }
            //populating list of academic year information into the academicList 
            academicYearViewModel.academicList = db.AcademicYear.ToList();
           
            return View(academicYearViewModel);
        }
        public ActionResult unitLeaderManagement()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            AcademicDirector ad = new AcademicDirector();
            RegisterTeacherViewModel rvm = new RegisterTeacherViewModel();
            //instantiating lists of the register teacher view model
            rvm.teacherList = new List<Models.Teacher>();
            rvm.retrevedTeacherList = new List<Models.Teacher>();
            //temporary lists to hold data for checking if a teacher in teacher's table hasa unit leader or teacher role.
            List<Models.Teacher> temp1 = new List<Models.Teacher>();
            List<Models.Teacher> temp2 = new List<Models.Teacher>();
            //populating list of teachers from teacher's table to temp1 list.
            temp1 = db.Teacher.ToList();
            foreach(var k in temp1)
            {
                //if the id of the teacher in teacher's table is teacher, it will add it to retreieved Teacher list for the view.
                if (ad.IsTeacherorUnitLeader(k.user.Id, "Teacher"))
                {
                    rvm.retrevedTeacherList.Add(k);
                } 

            }
            //populating list of teachers from teacher's table to temp2 list.
            temp2 = db.Teacher.ToList();
            //if the id of the teacher in teacher's table is a unit leader, it will add it to the teacherlist for the view.
            foreach (var k in temp2)
            {
                if (ad.IsTeacherorUnitLeader(k.user.Id, "UnitLeader"))
                {
                    rvm.teacherList.Add(k);
                }
            }
            return View(rvm);
        }
        [HttpPost]
        public ActionResult unitLeaderManagement(RegisterTeacherViewModel rvm,string selectToAssign,string assign,string update,string teacherID)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            AcademicDirector ad = new AcademicDirector();
            IdentityUserRole ir = new IdentityUserRole();
            var userStore = new ApplicationUserStore(db);
            var userManager = new ApplicationUserManager(userStore);
            ApplicationUser appUser = new ApplicationUser();
            //preparing 3 temp lists where two are for displaying list of Unit leaders and candidate teachers, while temp 3 is used for managing unit leader assignemnt update
            List<Models.Teacher> temp1 = new List<Models.Teacher>();
            List<Models.Teacher> temp2 = new List<Models.Teacher>();
            List<Models.Teacher> temp3 = new List<Models.Teacher>();
            ViewBag.Message = " ";
           

            rvm.teacherList = new List<Models.Teacher>();
            rvm.retrevedTeacherList = new List<Models.Teacher>();
            Models.Teacher retrieveAssignment = new Models.Teacher();
            //if select is selected for unit leader assignemnt
            
               
                //if assign is clicked
                if (assign != null)
                {
                //retreiving the teacher with the credentials recieved from the form using the register teaacher view model.
                retrieveAssignment = db.Teacher.Where(a => a.teacherId == teacherID).FirstOrDefault();
                //getting the teacher UID
               
                        teacherID = retrieveAssignment.user.Id;
                    
                    //searching the existence of theteacher from the ApplicationUser table 
                    appUser = userManager.FindById(teacherID);
                    //finding out role name and ID of the teacher
                    var oldRoleId = appUser.Roles.SingleOrDefault().RoleId;

                    var oldRoleName = db.Roles.SingleOrDefault(r => r.Id == oldRoleId).Name;
                    //collecting every teacher with the same grade of the current teacher
                    var teacherList = db.Teacher.Where(a => a.grade == retrieveAssignment.grade).ToList();
                    //status flag
                    int status = 0;
                    //iterating if the teachers id having the same grade are assigned unit leader status, if so disabling the previlige to add new academic leader for the specified grade.
                   /* foreach (var i in teacherList)
                    {
                        if (ad.IsTeacherorUnitLeader(i.teacherId, "UnitLeader"))
                        {
                            status = status + 1;
                        }
                    }*/
                    //if the status returns 0, meaning there is no teacher with the unit leader role, removing the teacher role of the selected teacher and assigning unitLeader role to it.
                    if (status == 0)
                    {
                        userManager.RemoveFromRole(appUser.Id, oldRoleName);
                        userManager.AddToRole(appUser.Id, "UnitLeader");
                          

                    //populating the list of teachers to temp3
                    temp3 = db.Teacher.ToList();
                    foreach (var k in temp3)
                    {
                        //   checking if the teacher has unitleader status, if so checking grade of the grade is same us the value recieved from the view model.
                        if (ad.IsTeacherorUnitLeader(k.user.Id, "UnitLeader"))
                        {
                            if (k.grade == retrieveAssignment.grade)
                            {
                                //updating the role of the teacher from unit leader to teacher.
                                userManager.RemoveFromRole(k.user.Id, "UnitLeader");
                                userManager.AddToRole(k.user.Id, "Teacher");
                                userManager.RemoveFromRole(teacherID, "Teacher");
                                userManager.AddToRole(teacherID, "UnitLeader");

                            }

                        }

                    }


                }
                else
                    {
                        ViewBag.Message = "Unit leader of the grade already exists.";
                    }

                }
            /*    //if update is clicked
                else if (update != null)
                {//retrieving the modifyable role user using the register view model and searching for the same credential in the teacher table.
                    retrieveAssignment = db.Teacher.Where(a => a.grade == rvm.grade && a.subject == rvm.subject && a.user.fullName == rvm.fullName).ToList();
                    //getting the teacher's user id using foreach loop
                    foreach (var k in retrieveAssignment)
                    {
                        teacherID = k.user.Id;
                    }
                    //populating the list of teachers to temp3
                    temp3 = db.Teacher.ToList();
                    foreach (var k in temp3)
                    {
                        //   checking if the teacher has unitleader status, if so checking grade of the grade is same us the value recieved from the view model.
                        if (ad.IsTeacherorUnitLeader(k.user.Id, "UnitLeader"))
                        {
                            if (k.grade == rvm.grade)
                            {
                                //updating the role of the teacher from unit leader to teacher.
                                userManager.RemoveFromRole(k.user.Id, "UnitLeader");
                                userManager.AddToRole(k.user.Id, "Teacher");
                                userManager.RemoveFromRole(teacherID, "Teacher");
                                userManager.AddToRole(teacherID, "UnitLeader");
                            }

                        }

                    }


                }*/
            
            //populaing list of teachers in temp 1, so if they have a teacher role, adding them to retreived teacher list.
            temp1 = db.Teacher.ToList();
            foreach (var k in temp1)
            {
                if (ad.IsTeacherorUnitLeader(k.user.Id, "Teacher"))
                {
                    rvm.retrevedTeacherList.Add(k);
                }

            }
            //populaing list of teachers in temp 2, so if they have a unit leader role, adding them to teacher list.
            temp2 = db.Teacher.ToList();
            foreach (var k in temp2)
            {
                if (ad.IsTeacherorUnitLeader(k.user.Id, "UnitLeader"))
                {
                    rvm.teacherList.Add(k);
                }
            }
            //returning the populated view model to the view.
            return View(rvm);
            
        }

        
        public ActionResult AddSection()
        {
            //object declaration
            AcademicDirector academicDirector = new AcademicDirector();
            SectionViewModel sectionViewModel = new SectionViewModel();

            sectionViewModel = academicDirector.populateFormData();
            return PartialView("AddSection",sectionViewModel);
        }

        [HttpPost]
        public ActionResult AddSection(SectionViewModel sectionViewModel, string letter, string teachers, string academicYears)
        {
            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            var userStore = new ApplicationUserStore(context);
            var userManager = new ApplicationUserManager(userStore);
            SectionViewModel sectionViewModelExtra = new SectionViewModel();
            AcademicDirector academicDirector = new AcademicDirector();
            Section section = new Section();

            //check if the record doesn't exist
            sectionViewModelExtra = academicDirector.searchSection(sectionViewModel.grade, letter);
            if (sectionViewModelExtra == null)
            {
                //get teacher Id and academic year Id 
                var findTeacher = context.Teacher.Where(t => t.user.fullName == teachers).FirstOrDefault();

                if (findTeacher.grade == sectionViewModel.grade)
                {
                    section.teacherId = findTeacher.teacherId;
                    section.academicYearId = academicYears;

                    //concatenate grade and section letter as a sectionName
                    section.sectionName = sectionViewModel.grade.ToString() + letter;

                    //save section record
                    context.Section.Add(section);
                    int sucess = context.SaveChanges();

                    if (sucess > 0)
                    {
                        //assign HomeRoom role for the selected teacher(remove teacher role)
                        userManager.RemoveFromRole(section.teacherId, "Teacher");
                        userManager.AddToRole(section.teacherId, "HomeRoom");

                        //success message
                        ViewBag.complete = "Section Created Successfully";
                    }
                    else
                    {
                        //error message
                        ViewBag.error = "Failed to Create Section!!";
                    }
                }
                else
                {
                    ViewBag.error = "Failed to Create Section. Teacher Not Teach on the Specified Grade";
                }
            }
            else
            {
                //error message
                ViewBag.error = "Section Already Exist. Either on Selected or Other Active Academic Year";
            }

            //populate selection list
            sectionViewModel = academicDirector.populateFormData();

            return PartialView("AddSection",sectionViewModel);
        }


        public ActionResult EditSection(string id)
        {
            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            SectionViewModel sectionViewModel = new SectionViewModel();
            AcademicDirector academicDirector = new AcademicDirector();
            var userStore = new ApplicationUserStore(context);
            var userManager = new ApplicationUserManager(userStore);
            sectionViewModel.teachers = new List<string>();

            //parse the passed id
            int Id = int.Parse(id);
            
            var getId = context.Section.Find(Id);

            int getGrade = int.Parse(getId.sectionName.Substring(0, getId.sectionName.Length-1));
            var gradeTeacher = context.Teacher.Where(t=>t.grade==getGrade).ToList();

            if (gradeTeacher.Count != 0)
            {
                foreach (var validTeachers in gradeTeacher)
                {
                    if (userManager.IsInRole(validTeachers.teacherId, "Teacher"))
                    {
                        sectionViewModel.teachers.Add(validTeachers.user.fullName);
                    }
                }
            }
            

            var getTeacher = context.Teacher.Where(t=>t.teacherId==getId.teacherId).FirstOrDefault();

            //include section home room in to the teacher selection

            sectionViewModel.teachers.Insert(0, getTeacher.user.fullName);
            sectionViewModel.ID = Id;
            ViewBag.teacher = getTeacher.user.fullName;

            //get acadamic year of the section record
            //ViewBag.academicYear = getId.academicYearId;

            return PartialView("EditSection",sectionViewModel);
        }

        [HttpPost]
        public ActionResult EditSection(SectionViewModel sectionViewModel,string teachers)
        {
            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            var userStore = new ApplicationUserStore(context);
            var userManager = new ApplicationUserManager(userStore);

            var sectionRecord = context.Section.Find(sectionViewModel.ID);

            //demote role from HomeRoom to Teacher
            userManager.RemoveFromRole(sectionRecord.teacherId, "HomeRoom");
            userManager.AddToRole(sectionRecord.teacherId, "Teacher");

            var newTeacherId = context.Teacher.Where(t => t.user.fullName == teachers).FirstOrDefault();

            //get section to update using sectionId
            sectionRecord.teacherId = newTeacherId.teacherId;

            //update section
            int sucess=context.SaveChanges();

            if (sucess > 0)
            {
                //promote role from Teacher to HomeRoom
                userManager.RemoveFromRole(newTeacherId.teacherId, "Teacher");
                userManager.AddToRole(newTeacherId.teacherId, "HomeRoom");

                ViewBag.complete = "Section Updated Successfully";
            }
            else
            {
                ViewBag.error = "Section Update Failed";
            }

            sectionViewModel.teachers.Clear();
            var getId = context.Section.Find(sectionViewModel.ID);

            int getGrade = int.Parse(getId.sectionName.Substring(0, getId.sectionName.Length - 1));
            var gradeTeacher = context.Teacher.Where(t => t.grade == getGrade).ToList();

            if (gradeTeacher.Count != 0)
            {
                foreach (var validTeachers in gradeTeacher)
                {
                    if (userManager.IsInRole(validTeachers.teacherId, "Teacher"))
                    {
                        sectionViewModel.teachers.Add(validTeachers.user.fullName);
                    }
                }
            }


            var getTeacher = context.Teacher.Where(t => t.teacherId == getId.teacherId).FirstOrDefault();

            //include section home room in to the teacher selection

            sectionViewModel.teachers.Insert(0, getTeacher.user.fullName);
            sectionViewModel.ID = sectionViewModel.ID;
            ViewBag.teacher = getTeacher.user.fullName;

            return PartialView("EditSection",sectionViewModel);
        }

        public ActionResult DeleteSection(string id)
        {
            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            SectionViewModel sectionViewModel = new SectionViewModel();

            int Id = int.Parse(id);

            var getSection = context.Section.Find(Id);

            var checkStudent = context.Student.Where(s => s.sectionName == getSection.sectionName && s.academicYearId == getSection.academicYearId).ToList();

            if (checkStudent.Count == 0)
            {
                ViewBag.message = "Are You Sure Do You Want to Delete This Section?";
            }
            else
            {
                ViewBag.error = "Unable to Delete Section. Students are Enrolled";
            }

            
            sectionViewModel.ID = Id;
            return PartialView("DeleteSection",sectionViewModel);
        }

        [HttpPost]
        public ActionResult DeleteSection(SectionViewModel sectionViewModel)
        {
            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            var userStore = new ApplicationUserStore(context);
            var userManager = new ApplicationUserManager(userStore);


            //delete section
            var sectionRecord = context.Section.Find(sectionViewModel.ID);

            //demote role from HoomRoom to Teacher
            userManager.RemoveFromRole(sectionRecord.teacherId, "HomeRoom");
            userManager.AddToRole(sectionRecord.teacherId, "Teacher");

            context.Section.Remove(sectionRecord);
            int success=context.SaveChanges();

            if (success > 0)
            {
                //successful message
                ViewBag.complete = "Section Deleted Successfully";
            }
            else
            {
                userManager.RemoveFromRole(sectionRecord.teacherId, "Teacher");
                userManager.AddToRole(sectionRecord.teacherId, "HomeRoom");
                ViewBag.posterror = "Section Deletion Failed!!";
            }
          
            return PartialView("DeleteSection");
        }

        public ActionResult SectionManagement()
        {
            //Academic Director object
            AcademicDirector academicDirector = new AcademicDirector();
            ApplicationDbContext context = new ApplicationDbContext();
            SectionViewModel sectionViewModel = new SectionViewModel();
            sectionViewModel.sections = new List<Section>();

            //viewbag element
            //ViewBag.search = false;
            //ViewBag.upHidden = "hidden";
            //ViewBag.read = false;

            var allAcadamicYears = context.AcademicYear.ToList();

            if (allAcadamicYears.Count != 0)
            {
                foreach (var getAcadamicYear in allAcadamicYears)
                {
                    //check today is in between start and end date of the specific academic year
                    if (!(DateTime.Compare(DateTime.Now, getAcadamicYear.durationStart) < 0 || DateTime.Compare(DateTime.Now, getAcadamicYear.durationEnd) > 0))
                    {
                        var listOfSections = context.Section.Where(s => s.academicYearId == getAcadamicYear.academicYearName).ToList();
                        if (listOfSections.Count > 0)
                        {
                            foreach (var getSection in listOfSections)
                            {
                                sectionViewModel.sections.Add(getSection);
                            }
                        }
                    }
                }
            }

            return View(sectionViewModel);
        
        }

        //[HttpPost]
        //public ActionResult SectionManagement(SectionViewModel sectionViewModel,string letter,string teachers,string academicYears,string add, string search, string update, string delete)
        //{
        //    //basic objects
        //    ApplicationDbContext context = new ApplicationDbContext();
        //    AcademicDirector academicDirector = new AcademicDirector();
        //    Section section = new Section();
        //    Models.Teacher teacher = new Models.Teacher();
        //    var userStore = new ApplicationUserStore(context);
        //    var userManager = new ApplicationUserManager(userStore);
        //    SectionViewModel sectionViewModelExtra = new SectionViewModel();

        //    //viewbag element used in UI
        //    ViewBag.search = false;
        //    ViewBag.upHidden = "hidden";
        //    ViewBag.read = false;

        //    if (ModelState.IsValid || (search != null && ModelState.IsValidField("grade")) || delete != null)
        //    {
        //        //check if Add button is clicked
        //        if (add != null)
        //        {
        //            //check if the record doesn't exist
        //            sectionViewModelExtra = academicDirector.searchSection(sectionViewModel.grade, letter);
        //            if (sectionViewModelExtra == null)
        //            {
        //                //get teacher Id and academic year Id 
        //                var findTeacher = context.Teacher.Where(t => t.user.fullName == teachers).FirstOrDefault();
        //                section.teacherId = findTeacher.teacherId;
        //                section.academicYearId = academicYears;

        //                //concatenate grade and section letter as a sectionName
        //                section.sectionName = sectionViewModel.grade.ToString() + letter;

        //                //save section record
        //                context.Section.Add(section);
        //                context.SaveChanges();

        //                //assign HomeRoom role for the selected teacher(remove teacher role)
        //                userManager.RemoveFromRole(section.teacherId, "Teacher");
        //                userManager.AddToRole(section.teacherId, "HomeRoom");

        //                //populate selection list
        //                sectionViewModel = academicDirector.populateFormData();

        //                //success message
        //                ViewBag.add = "Section Created Successfully";
        //            }
        //            else
        //            {
        //                //error message
        //                ViewBag.add = "The Section Already Exist on Either Selected or Other Active Academic Year";
        //            }
        //        }
        //        //check if search button is clicked
        //        else if (search != null)
        //        {
        //            //remove unwanted errors
        //            ModelState.Remove("teachers");
        //            ModelState.Remove("academicYears");

        //            //search section record
        //            sectionViewModelExtra = academicDirector.searchSection(sectionViewModel.grade, letter);

        //            //check if the record exists
        //            if (sectionViewModelExtra != null)
        //            {
        //                //viewbag attribute for UI usage
        //                ViewBag.search = true;
        //                ViewBag.upHidden = " ";
        //                ViewBag.read = true;

        //                //populate other selections in the sectionViewModel
        //                sectionViewModel = academicDirector.populateFormData();

        //                //include section home room in to the teacher selection
        //                foreach (var getTeacher in sectionViewModelExtra.teachers)
        //                {
        //                    sectionViewModel.teachers.Insert(0, getTeacher);
        //                    ViewBag.teacher = getTeacher;
        //                }
        //                //get acadamic year of the section record
        //                foreach (var getAcademicYear in sectionViewModelExtra.academicYears)
        //                {
        //                    ViewBag.academicYear = getAcademicYear;
        //                }
        //            }
        //            else
        //            {
        //                //record not found error message
        //                ViewBag.found = "Section Not Found";
        //                sectionViewModel = academicDirector.populateFormData();

        //            }
        //        }
        //        else if (update != null)
        //        {
        //            //check if it exists (also the year)and update
        //            var sectionRecord = context.Section.Where(s => s.sectionName == sectionViewModel.grade.ToString() + letter && s.academicYearId == academicYears).FirstOrDefault();

        //            if (sectionRecord != null)
        //            {
        //                //demote role from HomeRoom to Teacher
        //                userManager.RemoveFromRole(sectionRecord.teacherId, "HoomRoom");
        //                userManager.AddToRole(sectionRecord.teacherId, "Teacher");
        //                var newTeacherId = context.Teacher.Where(t => t.user.fullName == teachers).FirstOrDefault();

        //                //get section to update using sectionId
        //                var updateRecord = context.Section.Find(sectionRecord.sectionId);
        //                updateRecord.teacherId = newTeacherId.teacherId;

        //                //update section
        //                context.SaveChanges();

        //                //promote role from Teacher to HoomRoom
        //                userManager.RemoveFromRole(newTeacherId.teacherId, "Teacher");
        //                userManager.AddToRole(newTeacherId.teacherId, "HomeRoom");

        //            }

        //            //update success message 
        //            ViewBag.update = "Update Completed Successfully";
        //            ViewBag.read = false;
        //            ViewBag.upHidden = "hidden";

        //            //populate form selection options 
        //            sectionViewModel = academicDirector.populateFormData();

        //        }
        //        else if (delete != null)
        //        {
        //            //remove unwanted errors
        //            ModelState.Remove("teachers");
        //            ModelState.Remove("academicYears");

        //            //search section record
        //            sectionViewModelExtra = academicDirector.searchSection(sectionViewModel.grade, letter);

        //            if (sectionViewModelExtra != null)
        //            {
        //                //check if their are no students enrolled in the section
        //                Student student = new Student();
        //                var checkStudent = context.Student.Where(s => s.sectionName == sectionViewModelExtra.grade.ToString() + sectionViewModelExtra.letter[0].ToString() && s.academicYearId == sectionViewModelExtra.academicYears[0]).ToList();
        //                if (checkStudent.Count == 0)
        //                {
        //                    //delete section
        //                    var sectionRecord = context.Section.Where(s => s.sectionName == sectionViewModelExtra.grade.ToString() + sectionViewModelExtra.letter[0].ToString() && s.academicYearId == sectionViewModelExtra.academicYears[0]).FirstOrDefault();
        //                    context.Section.Remove(sectionRecord);
        //                    context.SaveChanges();

        //                    //demote role from HoomRoom to Teacher
        //                    userManager.RemoveFromRole(sectionRecord.teacherId, "HoomRoom");
        //                    userManager.AddToRole(sectionRecord.teacherId, "Teacher");

        //                    //successful message
        //                    ViewBag.delete = "Section Deleted Successfully";
        //                }
        //                else
        //                {
        //                    ViewBag.delete = "Unable to Delete Section. Students are Enrolled";
        //                }
        //            }
        //            else
        //            {
        //                //error message
        //                ViewBag.delete = "Unable to Delete Section Not Found";
        //            }
        //        }
        //    }

            

        //    return View(sectionViewModel);
        
        //}

        public ActionResult ScheduleManagement()
        {
            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            ScheduleViewModel scheduleViewModel = new ScheduleViewModel();
            scheduleViewModel.schedule = new List<Schedule>();

            //currentdate<>
            var schedule = context.Schedule.ToList();

            if (schedule.Count != 0)
            {
                foreach (var getSchedule in schedule)
                {
                    if (DateTime.Compare(DateTime.Now.Date, getSchedule.scheduleDate) < 0)
                    {
                        getSchedule.scheduleDate = getSchedule.scheduleDate;
                        scheduleViewModel.schedule.Add(getSchedule);
                    }
                }
            }

            return View(scheduleViewModel);
        }

        public ActionResult AddSchedule()
        {

            AddScheduleModal addScheduleModal = new AddScheduleModal();
            addScheduleModal.scheduleFor = new List<string>();

            addScheduleModal.scheduleFor.Add("Continious Assessment Test");
            addScheduleModal.scheduleFor.Add("Final Exam");
            addScheduleModal.scheduleFor.Add("Reassessment");

            return PartialView("AddSchedule",addScheduleModal);
        }

        [HttpPost]
        public ActionResult AddSchedule(AddScheduleModal addScheduleModal,string scheduleFor)
        {
            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            HomeroomTeacherMethod homeroomTeacherMethod = new HomeroomTeacherMethod();
            Schedule schedule = new Schedule();

            DateTime scheduleDate = DateTime.Parse(addScheduleModal.scheduleDate).Date;

            if (DateTime.Compare(DateTime.Now.Date, scheduleDate.Date) != 0)
            {

                var duplicate = context.Schedule.Where(s => s.grade == addScheduleModal.grade && s.subject.ToUpper() == addScheduleModal.subject.ToUpper() && s.scheduleDate == scheduleDate).FirstOrDefault();
                if (duplicate == null)
                {
                    Section identifyYear = new Section();

                    var allAcadamicYears = context.AcademicYear.ToList();

                    foreach (var getAcadamicYear in allAcadamicYears)
                    {
                        //check today is in between start and end date of the specific academic year
                        if (!(DateTime.Compare(DateTime.Now, getAcadamicYear.durationStart) < 0 || DateTime.Compare(DateTime.Now, getAcadamicYear.durationEnd) > 0))
                        {
                            identifyYear = context.Section.Where(s => s.sectionName.StartsWith(addScheduleModal.grade.ToString()) && s.academicYearId == getAcadamicYear.academicYearName).FirstOrDefault();
                            if (identifyYear != null)
                            {
                                break;
                            }
                        }
                    }

                    var getYear = context.AcademicYear.Find(identifyYear.academicYearId);
                    var quarter = homeroomTeacherMethod.whichQuarter(identifyYear.academicYearId);
                    DateTime quarterStart = DateTime.Now;
                    DateTime quarterEnd = DateTime.Now;

                    if (quarter == "Q1")
                    {
                        quarterStart = getYear.quarterOneStart.Date;
                        quarterEnd = getYear.quarterOneEnd.Date;
                    }
                    else if (quarter == "Q2")
                    {
                        quarterStart = getYear.quarterTwoStart.Date;
                        quarterEnd = getYear.quarterTwoEnd.Date;
                    }
                    else if (quarter == "Q3")
                    {
                        quarterStart = getYear.quarterThreeStart.Date;
                        quarterEnd = getYear.quarterThreeEnd.Date;
                    }
                    else if (quarter == "Q4")
                    {
                        quarterStart = getYear.quarterFourStart.Date;
                        quarterEnd = getYear.quarterFourEnd.Date;
                    }



                    if (scheduleFor == "Continious Assessment Test")
                    {
                        DateTime yesterday = scheduleDate.Date.Subtract(TimeSpan.FromDays(1));
                        DateTime tomorrow = scheduleDate.Date.Add(TimeSpan.FromDays(1));


                        var intervalOne = context.Schedule.Where(s => s.scheduleDate == yesterday && s.grade == addScheduleModal.grade).FirstOrDefault();
                        var intervalTwo = context.Schedule.Where(s => s.scheduleDate == tomorrow && s.grade == addScheduleModal.grade).FirstOrDefault();

                        if (intervalOne == null && intervalTwo == null)
                        {

                            if (!(DateTime.Compare(scheduleDate.Date, quarterStart) < 0 || DateTime.Compare(scheduleDate.Date, quarterEnd) > 0))
                            {
                                var percent = context.Schedule.Where(s => s.subject.ToUpper() == addScheduleModal.subject.ToUpper() && s.grade == addScheduleModal.grade && s.academicYear == getYear.academicYearName + "-" + quarter).ToList();
                                var assignmentPercent = context.Assignment.Where(a => a.yearlyQuarter == getYear.academicYearName + "-" + quarter && a.teacher.subject.ToUpper() == addScheduleModal.subject.ToUpper()).ToList();

                                if (percent.Count != 0 || assignmentPercent.Count != 0)
                                {
                                    int sum = 0;
                                    if (percent.Count != 0)
                                    {
                                        foreach (var getPercent in percent)
                                        {
                                            sum += getPercent.percentage;
                                        }
                                    }
                                    if (assignmentPercent.Count != 0)
                                    {
                                        foreach (var getPercent in assignmentPercent)
                                        {
                                            sum += getPercent.markPercentage;
                                        }
                                    }

                                    sum += addScheduleModal.percentage;

                                    if (sum <= 100)
                                    {
                                        schedule.academicYear = getYear.academicYearName + "-" + quarter;
                                        schedule.grade = addScheduleModal.grade;
                                        schedule.percentage = addScheduleModal.percentage;
                                        schedule.scheduleDate = scheduleDate.Date;
                                        schedule.scheduleFor = scheduleFor;
                                        schedule.subject = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(addScheduleModal.subject.ToLower());

                                        context.Schedule.Add(schedule);
                                        int result = context.SaveChanges();

                                        if (result > 0)
                                        {
                                            ViewBag.complete = "Schedule Successfully Scheduled";
                                        }
                                        else
                                        {
                                            ViewBag.error = "Failed To Create Schedule!!";
                                        }
                                    }
                                    else
                                    {
                                        ViewBag.error = "The Percentage Exceed Subject Score Limit in a Quarter";
                                    }
                                }
                                else
                                {
                                    schedule.academicYear = getYear.academicYearName + "-" + quarter;
                                    schedule.grade = addScheduleModal.grade;
                                    schedule.percentage = addScheduleModal.percentage;
                                    schedule.scheduleDate = scheduleDate.Date;
                                    schedule.scheduleFor = scheduleFor;
                                    schedule.subject = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(addScheduleModal.subject.ToLower());

                                    context.Schedule.Add(schedule);
                                    int result = context.SaveChanges();

                                    if (result > 0)
                                    {
                                        ViewBag.complete = "Schedule Successfully Scheduled";
                                    }
                                    else
                                    {
                                        ViewBag.error = "Failed To Create Schedule!!";
                                    }
                                }
                            }
                            else
                            {
                                ViewBag.error = "Schedule Out of Current Quarter Bound!!";
                            }

                        }
                        else
                        {
                            ViewBag.error = "Schedule Failed. Not Enough Interval!!";
                        }
                    }
                    else if (scheduleFor == "Reassessment")
                    {
                        //problem percentage and subject unknown critical


                        if (!(DateTime.Compare(scheduleDate.Date, quarterStart) < 0 || DateTime.Compare(scheduleDate.Date, quarterEnd) > 0))
                        {
                            schedule.academicYear = getYear.academicYearName + "-" + quarter;
                            schedule.grade = addScheduleModal.grade;
                            schedule.percentage = 0;
                            schedule.scheduleDate = scheduleDate.Date;
                            schedule.scheduleFor = scheduleFor;
                            schedule.subject = "All";

                            context.Schedule.Add(schedule);
                            int result = context.SaveChanges();

                            if (result > 0)
                            {
                                ViewBag.complete = "Schedule Successfully Scheduled";
                            }
                            else
                            {
                                ViewBag.error = "Failed To Create Schedule!!";
                            }
                        }
                        else
                        {
                            //error not in a current quarter
                            ViewBag.error = "Schedule Out of Current Quarter Bound!!";
                        }

                    }
                    else
                    {
                        //final exam

                        if (!(DateTime.Compare(scheduleDate.Date, quarterStart) < 0 || DateTime.Compare(scheduleDate.Date, quarterEnd) > 0))
                        {
                            var percent = context.Schedule.Where(s => s.subject.ToUpper() == addScheduleModal.subject.ToUpper() && s.grade == addScheduleModal.grade && s.academicYear == getYear.academicYearName + "-" + quarter).ToList();
                            var assignmentPercent = context.Assignment.Where(a => a.yearlyQuarter == getYear.academicYearName + "-" + quarter && a.teacher.subject.ToUpper() == addScheduleModal.subject.ToUpper()).ToList();

                            if (percent.Count != 0 || assignmentPercent.Count != 0)
                            {
                                int sum = 0;
                                if (percent.Count != 0)
                                {
                                    foreach (var getPercent in percent)
                                    {
                                        sum += getPercent.percentage;
                                    }
                                }
                                if (assignmentPercent.Count != 0)
                                {
                                    foreach (var getPercent in assignmentPercent)
                                    {
                                        sum += getPercent.markPercentage;
                                    }
                                }

                                sum += addScheduleModal.percentage;

                                if (sum <= 100)
                                {
                                    schedule.academicYear = getYear.academicYearName + "-" + quarter;
                                    schedule.grade = addScheduleModal.grade;
                                    schedule.percentage = addScheduleModal.percentage;
                                    schedule.scheduleDate = scheduleDate.Date;
                                    schedule.scheduleFor = scheduleFor;
                                    schedule.subject = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(addScheduleModal.subject.ToLower());

                                    context.Schedule.Add(schedule);
                                    int result = context.SaveChanges();

                                    if (result > 0)
                                    {
                                        ViewBag.complete = "Schedule Successfully Scheduled";
                                    }
                                    else
                                    {
                                        ViewBag.error = "Failed To Create Schedule!!";
                                    }
                                }
                                else
                                {
                                    //error percentage limit
                                    ViewBag.error = "The Percentage Exceed Subject Score Limit in a Quarter";
                                }
                            }
                            else
                            {
                                schedule.academicYear = getYear.academicYearName + "-" + quarter;
                                schedule.grade = addScheduleModal.grade;
                                schedule.percentage = addScheduleModal.percentage;
                                schedule.scheduleDate = scheduleDate.Date;
                                schedule.scheduleFor = scheduleFor;
                                schedule.subject = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(addScheduleModal.subject.ToLower());

                                context.Schedule.Add(schedule);
                                int result = context.SaveChanges();

                                if (result > 0)
                                {
                                    ViewBag.complete = "Schedule Successfully Scheduled";
                                }
                                else
                                {
                                    ViewBag.error = "Failed To Create Schedule!!";
                                }
                            }
                        }
                        else
                        {
                            //error not in a current quarter
                            ViewBag.error = "Schedule Out of Current Quarter Bound!!";
                        }


                    }
                }
                else
                {
                    //error duplicate
                    ViewBag.error = "Schedule Already Exist!!";
                }
            }
            else
            {
                //not today
                ViewBag.error = "You Can't Schedule For Today";
            }

            //check for duplicate same grade and subject on same day
            //check for yesterday and tomorrow for continious assessement same grade
            //reassesement subject all
            //check percentage 100%

            //get reassessement by combining attendance and schedule information (percentage)
            //if two exists add them up

            return PartialView("AddSchedule",addScheduleModal);
        }

        public ActionResult EditSchedule(string id)
        {
            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            AddScheduleModal addScheduleModal = new AddScheduleModal();
            addScheduleModal.scheduleFor = new List<string>();

            int Id = Int32.Parse(id);

            var schedule = context.Schedule.Find(Id);

            addScheduleModal.scheduleId = schedule.scheduleId;
            addScheduleModal.scheduleDate = schedule.scheduleDate.ToShortDateString();
            ViewBag.schedule = schedule.scheduleFor;
            addScheduleModal.grade = schedule.grade;
            addScheduleModal.percentage = schedule.percentage;
            addScheduleModal.subject = schedule.subject;

            addScheduleModal.scheduleFor.Add("Continious Assessment Test");
            addScheduleModal.scheduleFor.Add("Final Exam");
            addScheduleModal.scheduleFor.Add("Reassessment");



            return PartialView("EditSchedule",addScheduleModal);
        }

        [HttpPost]
        public ActionResult EditSchedule(AddScheduleModal addScheduleModal, string scheduleFor)
        {

            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            ApplicationDbContext contextUp = new ApplicationDbContext();
            HomeroomTeacherMethod homeroomTeacherMethod = new HomeroomTeacherMethod();
            Schedule schedule = contextUp.Schedule.Find(addScheduleModal.scheduleId);

            DateTime scheduleDate = DateTime.Parse(addScheduleModal.scheduleDate).Date;

            if (DateTime.Compare(DateTime.Now.Date, scheduleDate.Date) != 0)
            {

                var duplicate = context.Schedule.Where(s => s.grade == addScheduleModal.grade && s.subject.ToUpper() == addScheduleModal.subject.ToUpper() && s.scheduleDate == scheduleDate && s.scheduleId!=addScheduleModal.scheduleId).FirstOrDefault();
                if (duplicate == null)
                {
                    //Section identifyYear = new Section();

                    //var allAcadamicYears = context.AcademicYear.ToList();

                    //foreach (var getAcadamicYear in allAcadamicYears)
                    //{
                    //    //check today is in between start and end date of the specific academic year
                    //    if (!(DateTime.Compare(DateTime.Now, getAcadamicYear.durationStart) < 0 || DateTime.Compare(DateTime.Now, getAcadamicYear.durationEnd) > 0))
                    //    {
                    //        identifyYear = context.Section.Where(s => s.sectionName.StartsWith(addScheduleModal.grade.ToString()) && s.academicYearId == getAcadamicYear.academicYearName).FirstOrDefault();
                    //        if (identifyYear != null)
                    //        {
                    //            break;
                    //        }
                    //    }
                    //}

                    string[] yearQuarter = schedule.academicYear.Split('-');

                    var getYear = context.AcademicYear.Find(yearQuarter[0]);
                    var quarter = yearQuarter[1];
                    DateTime quarterStart = DateTime.Now;
                    DateTime quarterEnd = DateTime.Now;

                    if (quarter == "Q1")
                    {
                        quarterStart = getYear.quarterOneStart.Date;
                        quarterEnd = getYear.quarterOneEnd.Date;
                    }
                    else if (quarter == "Q2")
                    {
                        quarterStart = getYear.quarterTwoStart.Date;
                        quarterEnd = getYear.quarterTwoEnd.Date;
                    }
                    else if (quarter == "Q3")
                    {
                        quarterStart = getYear.quarterThreeStart.Date;
                        quarterEnd = getYear.quarterThreeEnd.Date;
                    }
                    else if (quarter == "Q4")
                    {
                        quarterStart = getYear.quarterFourStart.Date;
                        quarterEnd = getYear.quarterFourEnd.Date;
                    }



                    if (scheduleFor == "Continious Assessment Test")
                    {
                        DateTime yesterday = scheduleDate.Date.Subtract(TimeSpan.FromDays(1));
                        DateTime tomorrow = scheduleDate.Date.Add(TimeSpan.FromDays(1));


                        var intervalOne = context.Schedule.Where(s => s.scheduleDate == yesterday && s.grade == addScheduleModal.grade).FirstOrDefault();
                        var intervalTwo = context.Schedule.Where(s => s.scheduleDate == tomorrow && s.grade == addScheduleModal.grade).FirstOrDefault();

                        if (intervalOne == null && intervalTwo == null)
                        {

                            if (!(DateTime.Compare(scheduleDate.Date, quarterStart) < 0 || DateTime.Compare(scheduleDate.Date, quarterEnd) > 0))
                            {
                                var percent = context.Schedule.Where(s => s.subject.ToUpper() == addScheduleModal.subject.ToUpper() && s.grade == addScheduleModal.grade && s.academicYear == getYear.academicYearName + "-" + quarter && s.scheduleId!=addScheduleModal.scheduleId).ToList();
                                var assignmentPercent = context.Assignment.Where(a => a.yearlyQuarter == getYear.academicYearName + "-" + quarter && a.teacher.subject.ToUpper() == addScheduleModal.subject.ToUpper()).ToList();

                                if (percent.Count != 0 || assignmentPercent.Count != 0)
                                {
                                    int sum = 0;
                                    if (percent.Count != 0)
                                    {
                                        foreach (var getPercent in percent)
                                        {
                                            sum += getPercent.percentage;
                                        }
                                    }
                                    if (assignmentPercent.Count != 0)
                                    {
                                        foreach (var getPercent in assignmentPercent)
                                        {
                                            sum += getPercent.markPercentage;
                                        }
                                    }

                                    sum += addScheduleModal.percentage;

                                    if (sum <= 100)
                                    {
                                        schedule.percentage = addScheduleModal.percentage;
                                        schedule.scheduleDate = scheduleDate.Date;
                                        schedule.scheduleFor = scheduleFor;
                                        schedule.subject = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(addScheduleModal.subject.ToLower());
                                        
                                        int result = contextUp.SaveChanges();

                                        if (result > 0)
                                        {
                                            ViewBag.complete = "Schedule Updated Successfully";
                                        }
                                        else
                                        {
                                            ViewBag.error = "Failed To Update Schedule!!";
                                        }
                                    }
                                    else
                                    {
                                        ViewBag.error = "The Percentage Exceed Subject Score Limit in a Quarter";
                                    }
                                }
                                else
                                { 
                                    schedule.percentage = addScheduleModal.percentage;
                                    schedule.scheduleDate = scheduleDate.Date;
                                    schedule.scheduleFor = scheduleFor;
                                    schedule.subject = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(addScheduleModal.subject.ToLower());

                                    
                                    int result = contextUp.SaveChanges();

                                    if (result > 0)
                                    {
                                        ViewBag.complete = "Schedule Updated Successfully";
                                    }
                                    else
                                    {
                                        ViewBag.error = "Failed To Update Schedule!!";
                                    }
                                }
                            }
                            else
                            {
                                ViewBag.error = "Schedule Out of Current Quarter Bound!!";
                            }

                        }
                        else
                        {
                            ViewBag.error = "Update Failed. Not Enough Interval!!";
                        }
                    }
                    else if (scheduleFor == "Reassessment")
                    {
                        //problem percentage and subject unknown critical


                        if (!(DateTime.Compare(scheduleDate.Date, quarterStart) < 0 || DateTime.Compare(scheduleDate.Date, quarterEnd) > 0))
                        {
                            schedule.percentage = 0;
                            schedule.scheduleDate = scheduleDate.Date;
                            schedule.scheduleFor = scheduleFor;
                            schedule.subject = "All";
                           
                            int result = contextUp.SaveChanges();

                            if (result > 0)
                            {
                                ViewBag.complete = "Schedule Updated Successfully";
                            }
                            else
                            {
                                ViewBag.error = "Failed To Update Schedule!!";
                            }
                        }
                        else
                        {
                            //error not in a current quarter
                            ViewBag.error = "Schedule Out of Current Quarter Bound!!";
                        }

                    }
                    else
                    {
                        //final exam

                        if (!(DateTime.Compare(scheduleDate.Date, quarterStart) < 0 || DateTime.Compare(scheduleDate.Date, quarterEnd) > 0))
                        {
                            var percent = context.Schedule.Where(s => s.subject.ToUpper() == addScheduleModal.subject.ToUpper() && s.grade == addScheduleModal.grade && s.academicYear == getYear.academicYearName + "-" + quarter && s.scheduleId!=addScheduleModal.scheduleId).ToList();
                            var assignmentPercent = context.Assignment.Where(a => a.yearlyQuarter == getYear.academicYearName + "-" + quarter && a.teacher.subject.ToUpper() == addScheduleModal.subject.ToUpper()).ToList();

                            if (percent.Count != 0 || assignmentPercent.Count != 0)
                            {
                                int sum = 0;
                                if (percent.Count != 0)
                                {
                                    foreach (var getPercent in percent)
                                    {
                                        sum += getPercent.percentage;
                                    }
                                }
                                if (assignmentPercent.Count != 0)
                                {
                                    foreach (var getPercent in assignmentPercent)
                                    {
                                        sum += getPercent.markPercentage;
                                    }
                                }

                                sum += addScheduleModal.percentage;

                                if (sum <= 100)
                                {
                                    
                                    schedule.percentage = addScheduleModal.percentage;
                                    schedule.scheduleDate = scheduleDate.Date;
                                    schedule.scheduleFor = scheduleFor;
                                    schedule.subject = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(addScheduleModal.subject.ToLower());

                                    int result = contextUp.SaveChanges();

                                    if (result > 0)
                                    {
                                        ViewBag.complete = "Schedule Updated Successfully";
                                    }
                                    else
                                    {
                                        ViewBag.error = "Failed To Update Schedule!!";
                                    }
                                }
                                else
                                {
                                    //error percentage limit
                                    ViewBag.error = "The Percentage Exceed Subject Score Limit in a Quarter";
                                }
                            }
                            else
                            {
                                
                                schedule.percentage = addScheduleModal.percentage;
                                schedule.scheduleDate = scheduleDate.Date;
                                schedule.scheduleFor = scheduleFor;
                                schedule.subject = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(addScheduleModal.subject.ToLower());

                                int result = contextUp.SaveChanges();

                                if (result > 0)
                                {
                                    ViewBag.complete = "Schedule Updated Successfully";
                                }
                                else
                                {
                                    ViewBag.error = "Failed To Update Schedule!!";
                                }
                            }
                        }
                        else
                        {
                            //error not in a current quarter
                            ViewBag.error = "Schedule Out of Current Quarter Bound!!";
                        }


                    }
                }
                else
                {
                    //error duplicate
                    ViewBag.error = "Schedule Already Exist!!";
                }
            }
            else
            {
                //not today
                ViewBag.error = "You Can't Schedule For Today";
            }




            return PartialView("EditSchedule",addScheduleModal);
        }

        public ActionResult announcementManagement()
        {
            return View();
        }

        public ActionResult announcementManangement()
        {
            return View();
        }


    }
}

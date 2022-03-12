
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
using System.Web;

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
            //register teacher modal
            return PartialView("Register");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterTeacherModal registerTeacher)
        {
            
            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            var userStore = new ApplicationUserStore(context);
            var userManager = new ApplicationUserManager(userStore);
            Collection collection = new Collection();
            RegisterViewModel registerViewModel = new RegisterViewModel();
            Models.Teacher teacher = new Models.Teacher();
            AcademicDirector academicDirector = new AcademicDirector();

            //capitalize the first letters of fullname and subject
            registerTeacher.fullName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(registerTeacher.fullName.ToLower());
            registerTeacher.subject = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(registerTeacher.subject.ToLower());

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

                //generate random password
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
                            var revert1 = context.Teacher.Find(Id);
                            context.Teacher.Remove(revert1);
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

            return PartialView("Register", registerTeacher);
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
                updateTeacher.fullName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(teacher.user.fullName.ToLower()); ;
                updateTeacher.subject = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(teacher.subject.ToLower()); ;
                updateTeacher.grade = teacher.grade;

            }
            else
            {
                //error message for additional role
                ViewBag.geterror = "Update Failed. Teacher has Another Role Associated";
            }
            return PartialView("EditTeacher",updateTeacher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

            //check teacher doesn't have other assigned role
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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteTeacher(DeleteTeacherModal deleteTeacher)
        {
            AcademicDirector academicDirector = new AcademicDirector();
            Collection collection = new Collection();

            //delete teacher record
            Boolean delete= academicDirector.DeleteTeacher(deleteTeacher.Id);
            if (delete)
            {
                string status = await collection.DeleteUser(deleteTeacher.Id);

                if (status == "successful")
                {
                    //success message
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
            
            return PartialView("DeleteTeacher",deleteTeacher);
        }

        public ActionResult RegisterTeacher()
        {
            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            RegisterTeacherViewModel registerTeacherViewModel = new RegisterTeacherViewModel();
            registerTeacherViewModel.teacherList = context.Teacher.ToList();

            return View(registerTeacherViewModel);
        }

       public ActionResult registerRegistrar()
        {
            return PartialView("registerRegistrar");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
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
              rmv.fullName= System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(rmv.fullName.ToLower());
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
            rmvm= ad.listRegistrar();

            return PartialView("registerRegistrar",rmvm);
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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> manageRegistrar(RegistrarManagementViewModel rmv,string register,string delete,string theID){
            RegisterViewModel rv = new RegisterViewModel();
            Collection c = new Collection();
            RegistrarManagementViewModel rmvm = new RegistrarManagementViewModel();
            ApplicationDbContext db = new ApplicationDbContext();
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
                if (theID != null) {
                    ApplicationUser us = new ApplicationUser();
                    us = db.Users.Where(ax => ax.Id == theID).FirstOrDefault();
                    if (us != null) { 
                string status = await c.DeleteUser(theID);
                    }
                }
            }
            //refreshing the list of users with registrar role.
            rmvm= ad.listRegistrar();

            return View(rmvm);
        }
        public ActionResult registerAcademicYear()
        {
            return PartialView("registerAcademicYear");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
        [ValidateAntiForgeryToken]
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
                        ay1.durationStart = ay.durationStart;
                        ay1.durationEnd = ay.durationEnd;
                        ay1.quarterOneStart = ay.quarterOneStart;
                        ay1.quarterOneEnd = ay.quarterOneEnd;
                        ay1.quarterTwoStart = ay.quarterTwoStart;
                        ay1.quarterTwoEnd = ay.quarterTwoEnd;
                        ay1.quarterThreeStart = ay.quarterThreeStart;
                        ay1.quarterThreeEnd = ay.quarterThreeEnd;
                        ay1.quarterFourStart = ay.quarterFourStart;
                        ay1.quarterFourEnd = ay.quarterFourEnd;

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
        [ValidateAntiForgeryToken]
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
        [ValidateAntiForgeryToken]
        public ActionResult unitLeaderManagement(RegisterTeacherViewModel rvm,string selectToAssign,string assign,string delete,string teacherID)
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
            else if (delete != null)
            {
                retrieveAssignment = db.Teacher.Where(a => a.teacherId == teacherID).FirstOrDefault();
                userManager.RemoveFromRole(retrieveAssignment.teacherId, "UnitLeader");
                userManager.AddToRole(retrieveAssignment.teacherId, "Teacher");
            }
                
       
            
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

            //populate sectionViewModel
            sectionViewModel = academicDirector.populateFormData();
            return PartialView("AddSection",sectionViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSection(SectionViewModel sectionViewModel, string letter, string teachers, string academicYears)
        {
            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            var userStore = new ApplicationUserStore(context);
            var userManager = new ApplicationUserManager(userStore);
            SectionViewModel sectionViewModelExtra = new SectionViewModel();
            AcademicDirector academicDirector = new AcademicDirector();
            Section section = new Section();
            Section validAcademicYear = new Section();

            //check if the record doesn't exist
            sectionViewModelExtra = academicDirector.searchSection(sectionViewModel.grade, letter);
            if (sectionViewModelExtra == null)
            {
                //get teacher Id and academic year Id 
                var findTeacher = context.Teacher.Where(t => t.user.fullName == teachers).FirstOrDefault();

                if (findTeacher.grade == sectionViewModel.grade)
                {
                    //get all active acadamic years
                    var allAcadamicYears = context.AcademicYear.ToList();

                    if (allAcadamicYears.Count != 0)
                    {
                        foreach (var getAcadamicYear in allAcadamicYears)
                        {
                            //check today is in between start and end date of the specific academic year
                            if (!(DateTime.Compare(DateTime.Now, getAcadamicYear.durationStart) < 0 || DateTime.Compare(DateTime.Now, getAcadamicYear.durationEnd) > 0))
                            {
                                //check same identity sections exist in other academic years
                                validAcademicYear = context.Section.Where(s=>s.sectionName.StartsWith(sectionViewModel.grade.ToString()) && s.academicYearId!=academicYears).FirstOrDefault();
                                if (validAcademicYear != null)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    //check same identity sections exist in other academic years
                    if (validAcademicYear == null)
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
                        //error section with similar identities exist in another active acadamic year 
                        ViewBag.error = "Section Can Not be Created in This Academic Year";
                    }
                    
                }
                else
                {
                    //error message teacher grade not valid to create the specified section
                    ViewBag.error = "Failed to Create Section. Teacher Not Teach on the Specified Grade";
                }
            }
            else
            {
                //error message duplicate section
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

            //get teachers who teach on the specified grade
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

            //include section homeroom in to the teacher selection

            sectionViewModel.teachers.Insert(0, getTeacher.user.fullName);
            sectionViewModel.ID = Id;
            ViewBag.teacher = getTeacher.user.fullName;

            return PartialView("EditSection",sectionViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

                //success message
                ViewBag.complete = "Section Updated Successfully";
            }
            else
            {
                //error message
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

            //include section homeroom in to the teacher selection

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

            //check if section dont contain student
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
        [ValidateAntiForgeryToken]
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
                //remove homeroom from role
                userManager.RemoveFromRole(sectionRecord.teacherId, "Teacher");
                userManager.AddToRole(sectionRecord.teacherId, "HomeRoom");

                //error message
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

            //get active acadamic years
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

        public ActionResult ScheduleManagement()
        {
            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            ScheduleViewModel scheduleViewModel = new ScheduleViewModel();
            scheduleViewModel.schedule = new List<Schedule>();

            //get active schedules
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
            //object declaration
            AddScheduleModal addScheduleModal = new AddScheduleModal();
            addScheduleModal.scheduleFor = new List<string>();

            //populate dropdown
            addScheduleModal.scheduleFor.Add("Continious Assessment Test");
            addScheduleModal.scheduleFor.Add("Final Exam");
            addScheduleModal.scheduleFor.Add("Reassessment");

            addScheduleModal.scheduleDate = null;

            return PartialView("AddSchedule",addScheduleModal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSchedule(AddScheduleModal addScheduleModal,string scheduleFor)
        {
            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            HomeroomTeacherMethod homeroomTeacherMethod = new HomeroomTeacherMethod();
            Schedule schedule = new Schedule();

            DateTime scheduleDate = DateTime.Parse(addScheduleModal.scheduleDate).Date;
            //check schedule date is not on a weekend
            if(scheduleDate.DayOfWeek != DayOfWeek.Sunday || scheduleDate.DayOfWeek != DayOfWeek.Saturday)
            {
                //check if schedule date is not current date
                if (DateTime.Compare(DateTime.Now.Date, scheduleDate.Date) != 0)
                {
                    //check schedule for duplicate entry
                    var duplicate = context.Schedule.Where(s => s.grade == addScheduleModal.grade && s.subject.ToUpper() == addScheduleModal.subject.ToUpper() && s.scheduleDate == scheduleDate).FirstOrDefault();
                    if (duplicate == null)
                    {
                        Section identifyYear = new Section();

                        //get active academic year
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

                        //get start and end dates of the quarter
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


                        //schedule for Continious Assessment
                        if (scheduleFor == "Continious Assessment Test")
                        {
                            //get yesterday and tomorrow date from schedule date
                            DateTime yesterday = scheduleDate.Date.Subtract(TimeSpan.FromDays(1));
                            DateTime tomorrow = scheduleDate.Date.Add(TimeSpan.FromDays(1));

                            //check if their are schedules before and after the schedule date   
                            var intervalOne = context.Schedule.Where(s => s.scheduleDate == yesterday && s.grade == addScheduleModal.grade).FirstOrDefault();
                            var intervalTwo = context.Schedule.Where(s => s.scheduleDate == tomorrow && s.grade == addScheduleModal.grade).FirstOrDefault();

                            if (intervalOne == null && intervalTwo == null)
                            {
                                //check if schedule date is in the quarter duration
                                if (!(DateTime.Compare(scheduleDate.Date, quarterStart) < 0 || DateTime.Compare(scheduleDate.Date, quarterEnd) > 0))
                                {
                                    //get percentage and subject details
                                    var percent = context.Schedule.Where(s => s.subject.ToUpper() == addScheduleModal.subject.ToUpper() && s.grade == addScheduleModal.grade && s.academicYear == getYear.academicYearName + "-" + quarter).ToList();
                                    var assignmentPercent = context.Assignment.Where(a => a.yearlyQuarter == getYear.academicYearName + "-" + quarter && a.teacher.subject.ToUpper() == addScheduleModal.subject.ToUpper()).ToList();
                                    var subject = context.Teacher.Where(s => s.grade == addScheduleModal.grade && s.subject.ToUpper() == addScheduleModal.subject.ToUpper()).FirstOrDefault();

                                    //check if their is a teacher who teaches the specified subject
                                    if (subject != null)
                                    {
                                        //get previous assesement percentages
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

                                            //check previous and current scheduled assesement not excced 100%
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
                                                    //success message
                                                    ViewBag.complete = "Schedule Successfully Scheduled";
                                                }
                                                else
                                                {
                                                    //error message
                                                    ViewBag.error = "Failed To Create Schedule!!";
                                                }
                                            }
                                            else
                                            {
                                                //error message percentage excced 100% limit
                                                ViewBag.error = "The Percentage Exceed Subject Score Limit in a Quarter";
                                            }
                                        }
                                        else
                                        {
                                            //populate schedule object
                                            schedule.academicYear = getYear.academicYearName + "-" + quarter;
                                            schedule.grade = addScheduleModal.grade;
                                            schedule.percentage = addScheduleModal.percentage;
                                            schedule.scheduleDate = scheduleDate.Date;
                                            schedule.scheduleFor = scheduleFor;
                                            schedule.subject = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(addScheduleModal.subject.ToLower());

                                            //record schedule
                                            context.Schedule.Add(schedule);
                                            int result = context.SaveChanges();

                                            if (result > 0)
                                            {
                                                //success message
                                                ViewBag.complete = "Schedule Successfully Scheduled";
                                            }
                                            else
                                            {
                                                //error message
                                                ViewBag.error = "Failed To Create Schedule!!";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //error no one teaches the subject
                                        ViewBag.error = "No Teacher Teaches The Specified Subject";
                                    }

                                }
                                else
                                {
                                    //error message schedule date out of quarter bound
                                    ViewBag.error = "Schedule Out of Current Quarter Bound!!";
                                }

                            }
                            else
                            {
                                //error message one day gap not satisfied 
                                ViewBag.error = "Schedule Failed. Not Enough Interval!!";
                            }
                        }//schedule for reassesement
                        else if (scheduleFor == "Reassessment")
                        {
                            //check schedule is scheduled in the quarter duration scope
                            if (!(DateTime.Compare(scheduleDate.Date, quarterStart) < 0 || DateTime.Compare(scheduleDate.Date, quarterEnd) > 0))
                            {
                                //populate schedule object
                                schedule.academicYear = getYear.academicYearName + "-" + quarter;
                                schedule.grade = addScheduleModal.grade;
                                schedule.percentage = 0;
                                schedule.scheduleDate = scheduleDate.Date;
                                schedule.scheduleFor = scheduleFor;
                                schedule.subject = "All";

                                //record schedule
                                context.Schedule.Add(schedule);
                                int result = context.SaveChanges();

                                if (result > 0)
                                {
                                    //success message
                                    ViewBag.complete = "Schedule Successfully Scheduled";
                                }
                                else
                                {
                                    //error message
                                    ViewBag.error = "Failed To Create Schedule!!";
                                }
                            }
                            else
                            {
                                //error not in a current quarter
                                ViewBag.error = "Schedule Out of Current Quarter Bound!!";
                            }

                        }//schedule final exam
                        else
                        {
                            //check schedule date is in a quarter duration scope 

                            if (!(DateTime.Compare(scheduleDate.Date, quarterStart) < 0 || DateTime.Compare(scheduleDate.Date, quarterEnd) > 0))
                            {
                                //get percent and subject information
                                var percent = context.Schedule.Where(s => s.subject.ToUpper() == addScheduleModal.subject.ToUpper() && s.grade == addScheduleModal.grade && s.academicYear == getYear.academicYearName + "-" + quarter).ToList();
                                var assignmentPercent = context.Assignment.Where(a => a.yearlyQuarter == getYear.academicYearName + "-" + quarter && a.teacher.subject.ToUpper() == addScheduleModal.subject.ToUpper()).ToList();
                                var subject = context.Teacher.Where(s => s.grade == addScheduleModal.grade && s.subject.ToUpper() == addScheduleModal.subject.ToUpper()).FirstOrDefault();

                                //check if teacher exist who teaches the specified subject
                                if (subject != null)
                                {
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

                                        //check if percentage sum not excced 100%
                                        if (sum <= 100)
                                        {
                                            //populate schedule object
                                            schedule.academicYear = getYear.academicYearName + "-" + quarter;
                                            schedule.grade = addScheduleModal.grade;
                                            schedule.percentage = addScheduleModal.percentage;
                                            schedule.scheduleDate = scheduleDate.Date;
                                            schedule.scheduleFor = scheduleFor;
                                            schedule.subject = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(addScheduleModal.subject.ToLower());

                                            //record schedule
                                            context.Schedule.Add(schedule);
                                            int result = context.SaveChanges();

                                            if (result > 0)
                                            {
                                                //success message
                                                ViewBag.complete = "Schedule Successfully Scheduled";
                                            }
                                            else
                                            {
                                                //error message
                                                ViewBag.error = "Failed To Create Schedule!!";
                                            }
                                        }
                                        else
                                        {
                                            //error percentage limit exceed 100%
                                            ViewBag.error = "The Percentage Exceed Subject Score Limit in a Quarter";
                                        }
                                    }
                                    else
                                    {
                                        //populate schedule subject
                                        schedule.academicYear = getYear.academicYearName + "-" + quarter;
                                        schedule.grade = addScheduleModal.grade;
                                        schedule.percentage = addScheduleModal.percentage;
                                        schedule.scheduleDate = scheduleDate.Date;
                                        schedule.scheduleFor = scheduleFor;
                                        schedule.subject = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(addScheduleModal.subject.ToLower());

                                        //record schedule
                                        context.Schedule.Add(schedule);
                                        int result = context.SaveChanges();

                                        if (result > 0)
                                        {
                                            //success message
                                            ViewBag.complete = "Schedule Successfully Scheduled";
                                        }
                                        else
                                        {
                                            //error message
                                            ViewBag.error = "Failed To Create Schedule!!";
                                        }
                                    }
                                }
                                else
                                {
                                    //error no one teaches the subject
                                    ViewBag.error = "No Teacher Teaches The Specified Subject";
                                }

                            }
                            else
                            {
                                //error not in a current quarter duration scope
                                ViewBag.error = "Schedule Out of Current Quarter Bound!!";
                            }


                        }
                    }
                    else
                    {
                        //error message duplicate schedule
                        ViewBag.error = "Schedule Already Exist!!";
                    }
                }
                else
                {
                    //error message can not schedule on the current date
                    ViewBag.error = "You Can't Schedule For Today";
                }
            }
            else
            {
                //error message weekend
                ViewBag.error = "Can Not Schedule at Weekend Dates";
            }
            
            addScheduleModal.scheduleFor.Clear();

            //populate dropdown
            addScheduleModal.scheduleFor.Add("Continious Assessment Test");
            addScheduleModal.scheduleFor.Add("Final Exam");
            addScheduleModal.scheduleFor.Add("Reassessment");

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

            //populate addScheduleObject 
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
        [ValidateAntiForgeryToken]
        public ActionResult EditSchedule(AddScheduleModal addScheduleModal, string scheduleFor)
        {

            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            ApplicationDbContext contextUp = new ApplicationDbContext();
            HomeroomTeacherMethod homeroomTeacherMethod = new HomeroomTeacherMethod();
            Schedule schedule = contextUp.Schedule.Find(addScheduleModal.scheduleId);

            DateTime scheduleDate = DateTime.Parse(addScheduleModal.scheduleDate).Date;

            if(scheduleDate.DayOfWeek != DayOfWeek.Sunday || scheduleDate.DayOfWeek != DayOfWeek.Saturday)
            {
                //check schedule date is not current date
                if (DateTime.Compare(DateTime.Now.Date, scheduleDate.Date) != 0)
                {
                    //check if similar schedule exists
                    var duplicate = context.Schedule.Where(s => s.grade == addScheduleModal.grade && s.subject.ToUpper() == addScheduleModal.subject.ToUpper() && s.scheduleDate == scheduleDate && s.scheduleId != addScheduleModal.scheduleId).FirstOrDefault();
                    if (duplicate == null)
                    {
                        //get Acadamic year and current quarter
                        string[] yearQuarter = schedule.academicYear.Split('-');

                        var getYear = context.AcademicYear.Find(yearQuarter[0]);
                        var quarter = yearQuarter[1];
                        DateTime quarterStart = DateTime.Now;
                        DateTime quarterEnd = DateTime.Now;

                        //get start and end dates of current quarter
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


                        //update to Continious Assessment
                        if (scheduleFor == "Continious Assessment Test")
                        {
                            //get yeasterday and tomoorow dates form schedule date
                            DateTime yesterday = scheduleDate.Date.Subtract(TimeSpan.FromDays(1));
                            DateTime tomorrow = scheduleDate.Date.Add(TimeSpan.FromDays(1));

                            //check one day interval
                            var intervalOne = context.Schedule.Where(s => s.scheduleDate == yesterday && s.grade == addScheduleModal.grade && s.scheduleId != addScheduleModal.scheduleId).FirstOrDefault();
                            var intervalTwo = context.Schedule.Where(s => s.scheduleDate == tomorrow && s.grade == addScheduleModal.grade && s.scheduleId != addScheduleModal.scheduleId).FirstOrDefault();

                            if (intervalOne == null && intervalTwo == null)
                            {
                                //check schedule is with in quarter duartion bound
                                if (!(DateTime.Compare(scheduleDate.Date, quarterStart) < 0 || DateTime.Compare(scheduleDate.Date, quarterEnd) > 0))
                                {
                                    //get percentage and subject information
                                    var percent = context.Schedule.Where(s => s.subject.ToUpper() == addScheduleModal.subject.ToUpper() && s.grade == addScheduleModal.grade && s.academicYear == getYear.academicYearName + "-" + quarter && s.scheduleId != addScheduleModal.scheduleId).ToList();
                                    var assignmentPercent = context.Assignment.Where(a => a.yearlyQuarter == getYear.academicYearName + "-" + quarter && a.teacher.subject.ToUpper() == addScheduleModal.subject.ToUpper()).ToList();
                                    var subject = context.Teacher.Where(s => s.grade == addScheduleModal.grade && s.subject.ToUpper() == addScheduleModal.subject.ToUpper()).FirstOrDefault();

                                    //check if teaher exist who teaches the specified subject
                                    if (subject != null)
                                    {
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

                                            //check sum of assesement percentage not exceed 100%
                                            if (sum <= 100)
                                            {
                                                //populate the new data to schedule object
                                                schedule.percentage = addScheduleModal.percentage;
                                                schedule.scheduleDate = scheduleDate.Date;
                                                schedule.scheduleFor = scheduleFor;
                                                schedule.subject = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(addScheduleModal.subject.ToLower());

                                                //update schedule
                                                int result = contextUp.SaveChanges();

                                                if (result > 0)
                                                {
                                                    //success message
                                                    ViewBag.complete = "Schedule Updated Successfully";
                                                }
                                                else
                                                {
                                                    //error message
                                                    ViewBag.error = "Failed To Update Schedule!!";
                                                }
                                            }
                                            else
                                            {
                                                //error message sum of percentage exceed 100%
                                                ViewBag.error = "The Percentage Exceed Subject Score Limit in a Quarter";
                                            }
                                        }
                                        else
                                        {
                                            //populate the new data to schedule object
                                            schedule.percentage = addScheduleModal.percentage;
                                            schedule.scheduleDate = scheduleDate.Date;
                                            schedule.scheduleFor = scheduleFor;
                                            schedule.subject = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(addScheduleModal.subject.ToLower());

                                            //update schedule
                                            int result = contextUp.SaveChanges();

                                            if (result > 0)
                                            {
                                                //successful message
                                                ViewBag.complete = "Schedule Updated Successfully";
                                            }
                                            else
                                            {
                                                //error message
                                                ViewBag.error = "Failed To Update Schedule!!";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //error no one teaches the subject
                                        ViewBag.error = "No Teacher Teaches The Specified Subject";
                                    }
                                }
                                else
                                {
                                    //error message schedule date out of quarter duration
                                    ViewBag.error = "Schedule Out of Current Quarter Bound!!";
                                }

                            }
                            else
                            {
                                //error message one day interval not satsfied
                                ViewBag.error = "Update Failed. Not Enough Interval!!";
                            }
                        }
                        else if (scheduleFor == "Reassessment")
                        {
                            //check schedule date is with in quarter bound
                            if (!(DateTime.Compare(scheduleDate.Date, quarterStart) < 0 || DateTime.Compare(scheduleDate.Date, quarterEnd) > 0))
                            {
                                //populate schedule object
                                schedule.percentage = 0;
                                schedule.scheduleDate = scheduleDate.Date;
                                schedule.scheduleFor = scheduleFor;
                                schedule.subject = "All";

                                //update schedule
                                int result = contextUp.SaveChanges();

                                if (result > 0)
                                {
                                    //success message
                                    ViewBag.complete = "Schedule Updated Successfully";
                                }
                                else
                                {
                                    //error message
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
                            //check Schedule date is with in quarter bound
                            if (!(DateTime.Compare(scheduleDate.Date, quarterStart) < 0 || DateTime.Compare(scheduleDate.Date, quarterEnd) > 0))
                            {
                                //get percentage information
                                var percent = context.Schedule.Where(s => s.subject.ToUpper() == addScheduleModal.subject.ToUpper() && s.grade == addScheduleModal.grade && s.academicYear == getYear.academicYearName + "-" + quarter && s.scheduleId != addScheduleModal.scheduleId).ToList();
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

                                    //check if sum does not exceed 100%
                                    if (sum <= 100)
                                    {
                                        //populate schedule object
                                        schedule.percentage = addScheduleModal.percentage;
                                        schedule.scheduleDate = scheduleDate.Date;
                                        schedule.scheduleFor = scheduleFor;
                                        schedule.subject = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(addScheduleModal.subject.ToLower());

                                        int result = contextUp.SaveChanges();

                                        if (result > 0)
                                        {
                                            //success message
                                            ViewBag.complete = "Schedule Updated Successfully";
                                        }
                                        else
                                        {
                                            //error message
                                            ViewBag.error = "Failed To Update Schedule!!";
                                        }
                                    }
                                    else
                                    {
                                        //error percentage limit excced 100%
                                        ViewBag.error = "The Percentage Exceed Subject Score Limit in a Quarter";
                                    }
                                }
                                else
                                {
                                    //populate schedule object
                                    schedule.percentage = addScheduleModal.percentage;
                                    schedule.scheduleDate = scheduleDate.Date;
                                    schedule.scheduleFor = scheduleFor;
                                    schedule.subject = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(addScheduleModal.subject.ToLower());

                                    //update schedule
                                    int result = contextUp.SaveChanges();

                                    if (result > 0)
                                    {
                                        //success message
                                        ViewBag.complete = "Schedule Updated Successfully";
                                    }
                                    else
                                    {
                                        //error message
                                        ViewBag.error = "Failed To Update Schedule!!";
                                    }
                                }
                            }
                            else
                            {
                                //error message Schedule date not in the quarter duration boundary
                                ViewBag.error = "Schedule Out of Current Quarter Bound!!";
                            }


                        }
                    }
                    else
                    {
                        //error message duplicate schedule
                        ViewBag.error = "Schedule Already Exist!!";
                    }
                }
                else
                {
                    //error message can not schedule for current date
                    ViewBag.error = "You Can't Schedule For Today";
                }
            }
            else
            {
                //error message weekend
                ViewBag.error = "Can Not Schedule at Weekend Dates";
            }

            

            addScheduleModal.scheduleFor.Clear();

            //populate dropdown
            addScheduleModal.scheduleFor.Add("Continious Assessment Test");
            addScheduleModal.scheduleFor.Add("Final Exam");
            addScheduleModal.scheduleFor.Add("Reassessment");

            return PartialView("EditSchedule",addScheduleModal);
        }
        [HttpGet]
        public ActionResult addAnnouncement(string anId, string txtAddAnnouncement,string txtAddAnnouncement1)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string[] announcementItems = new string[1];
            announcementItems = txtAddAnnouncement.Split('-');
            int grade = int.Parse(txtAddAnnouncement);
            string gr1 = txtAddAnnouncement;
            string academicYear = txtAddAnnouncement1;
            registerAnnouncementViewModel rvm = new registerAnnouncementViewModel();
            rvm.sectionList = new List<Section>();
            rvm.sectionList = db.Section.Where(ax => ax.sectionName.StartsWith(gr1) && ax.academicYearId == academicYear).ToList();
            rvm.studentList = new List<Student>();
            rvm.studentList = db.Student.Where(ax => ax.academicYearId == academicYear && ax.sectionName.StartsWith(gr1)).ToList();
            rvm.grade = grade;
            rvm.academicYear = academicYear;
                       
            return View(rvm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult addAnnouncement(string selectedSections,string fileName, HttpPostedFileBase file,registerAnnouncementViewModel avm,string add)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Announcement ans = new Announcement();
            gradeAnnouncement gans = new gradeAnnouncement();
            avm.sectionList = new List<Section>();
            avm.studentList = new List<Student>();
            avm.gradeList = new List<gradeViewModel>();
            string grades = avm.grade.ToString();
            avm.announcementList = new List<Announcement>();
            announcementViewModel avm2 = new announcementViewModel();
            avm2.gradeList = new List<gradeViewModel>();
            avm2.announcementList = new List<Announcement>();
            if (add != null)
            {
                if (ModelState.IsValid)
                {
                    if ((file != null && file.ContentType == "application/pdf")) { 
                    if (selectedSections != null && selectedSections != "" && selectedSections != "*")
                    {
                        String firstSeries = selectedSections.Substring(1, selectedSections.Length - 2);

                        List<string> memList = new List<string>();
                        memList = firstSeries.Split('-').ToList();
                        if (memList.Count == db.Section.Where(ax => ax.sectionName.StartsWith(grades) && ax.academicYearId == avm.academicYear).Count())
                        {

                            ans.announcementTitle = avm.announcementTitle;
                            ans.announcementType = "grade";
                            ans.announcementContent = avm.announcementContent;
                            ans.endDate = Convert.ToDateTime(avm.endDate).Date;
                            ans.postDate = DateTime.Now.Date;
                            if (file != null)
                            {
                                int length = file.ContentLength;
                                byte[] upload = new byte[length];
                                file.InputStream.Read(upload, 0, length);
                                ans.announcementDocument = upload;
                                ans.filName = fileName;
                            }
                            var ayr = db.Announcement.Where(ax => ax.announcementType == "grade" && ax.announcementContent == ans.announcementContent && ax.announcementTitle == ans.announcementTitle && ax.endDate == ans.endDate).FirstOrDefault();
                            if (ayr == null)
                            {
                                db.Announcement.Add(ans);
                                db.SaveChanges();

                                var axr = db.Announcement.Where(ax => ax.announcementType == "grade" && ax.announcementContent == ans.announcementContent && ax.announcementTitle == ans.announcementTitle && ax.endDate == ans.endDate).FirstOrDefault();
                                gans.announcementId = axr.announcementID;
                                gans.grade = avm.grade;
                                db.gradeAnnouncements.Add(gans);
                                db.SaveChanges();
                            }
                            else
                            {
                                ViewBag.existsMessage = "Announcement Exists";
                                goto x;
                            }


                        }
                        else
                        {
                            ans.announcementTitle = avm.announcementTitle;
                            ans.announcementType = "section";
                            ans.announcementContent = avm.announcementContent;
                            ans.endDate = Convert.ToDateTime(avm.endDate).Date;
                            ans.postDate = DateTime.Now.Date;
                            if (file != null)
                            {
                                int length = file.ContentLength;
                                byte[] upload = new byte[length];
                                file.InputStream.Read(upload, 0, length);
                                ans.announcementDocument = upload;
                                ans.filName = fileName;
                            }
                            var ayr = db.Announcement.Where(ax => ax.announcementType == "section" && ax.announcementContent == ans.announcementContent && ax.announcementTitle == ans.announcementTitle && ax.endDate == ans.endDate).FirstOrDefault();
                            if (ayr == null)
                            {
                                db.Announcement.Add(ans);
                                db.SaveChanges();
                                var axr = db.Announcement.Where(ax => ax.announcementType == "section" && ax.announcementContent == ans.announcementContent && ax.announcementTitle == ans.announcementTitle && ax.endDate == ans.endDate).FirstOrDefault();
                                foreach (var k in memList)
                                {
                                    sectionAnnouncement sans = new sectionAnnouncement();
                                    sans.announcementId = axr.announcementID;
                                    var theSec = db.Section.Where(ax => ax.sectionName == k && ax.academicYearId == avm.academicYear).FirstOrDefault();
                                    sans.sectionId = theSec.sectionId;
                                    db.sectionAnnouncement.Add(sans);
                                    db.SaveChanges();
                                }
                            }
                            else
                            {
                                ViewBag.existsMessage = "Announcement Exists";
                                goto x;
                            }

                        }
                        HomeroomTeacherMethod htm = new HomeroomTeacherMethod();
                        List<Section> secList = new List<Section>();
                        List<Section> tempSecList = new List<Section>();
                        secList = db.Section.ToList();
                        if (secList != null)
                        {
                            foreach (var k in secList)
                            {
                                if (htm.isInAcademicYear(k.academicYearId))
                                {
                                    tempSecList.Add(k);
                                }
                            }
                            if (tempSecList != null)
                            {
                                foreach (var k in tempSecList)
                                {
                                    int xr = int.Parse(k.sectionName.Substring(0, k.sectionName.Length - 1));
                                    string xr1 = xr.ToString();
                                    var gr = avm2.gradeList.Where(ax => ax.grade == xr).FirstOrDefault();
                                    if (gr == null)
                                    {
                                        gradeViewModel gvm = new gradeViewModel();
                                        gvm.grade = xr;
                                        List<Student> stdList = new List<Student>();
                                        stdList = db.Student.Where(ax => ax.academicYearId == k.academicYearId && ax.sectionName.StartsWith(xr1)).ToList();
                                        gvm.numberOfStudents = stdList.Count();
                                        gvm.numberoFSections = tempSecList.Where(ax => ax.sectionName.StartsWith(xr1)).Count();
                                        gvm.academicYear = k.academicYearId;
                                        avm2.gradeList.Add(gvm);
                                    }
                                }
                            }
                        }
                        List<Announcement> annList = new List<Announcement>();
                        annList = db.Announcement.ToList();
                        if (annList != null)
                        {
                            foreach (var k in annList)
                            {
                                if (DateTime.Compare(DateTime.Now.Date, k.endDate.Date) <= 0)
                                {
                                    avm2.announcementList.Add(k);
                                }
                            }
                        }
                        return View("announcementManagement", avm2);


                    }
                }
                    else
                    {
                        ViewBag.incorrectFileFormat = "Invalid file format. Please upload pdf file";
                    }
            }
            }
            x:
            avm.sectionList = db.Section.Where(ax => ax.sectionName.StartsWith(grades) && ax.academicYearId == avm.academicYear).ToList();
            avm.studentList = db.Student.Where(ax => ax.academicYearId == avm.academicYear && ax.sectionName.StartsWith(grades)).ToList();
            
            
            return View(avm);
        }
        [HttpGet]
        public ActionResult addStudentAnnouncement(string txtAddAnnouncement, string anId,string add)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            int studentId = int.Parse(txtAddAnnouncement);
            Student s = new Student();
            s = db.Student.Where(ax => ax.studentId == studentId).FirstOrDefault();
            registerAnnouncementViewModel avm = new registerAnnouncementViewModel();
            avm.studentId = s.studentId;
            avm.studentName = s.fullName;
           
            return View(avm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult addStudentAnnouncement(string fileName, HttpPostedFileBase file, registerAnnouncementViewModel avm, string add)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Announcement ans = new Announcement();
            studentAnnouncement sA = new studentAnnouncement();
            avm.gradeList = new List<gradeViewModel>();
            avm.announcementList = new List<Announcement>();
            announcementViewModel avm2 = new announcementViewModel();
            avm2.gradeList= new List<gradeViewModel>();
            avm2.announcementList = new List<Announcement>();
            if (add != null)
            {
                if (ModelState.IsValid)
                {
                    if ((file != null && file.ContentType == "application/pdf"))
                    { 
                    ans.announcementTitle = avm.announcementTitle;
                    ans.announcementType = "student";
                    ans.announcementContent = avm.announcementContent;
                    ans.postDate = DateTime.Now.Date;
                    ans.endDate = Convert.ToDateTime(avm.endDate).Date;
                    if (file != null)
                    {
                        int length = file.ContentLength;
                        byte[] upload = new byte[length];
                        file.InputStream.Read(upload, 0, length);
                        ans.announcementDocument = upload;
                        ans.filName = fileName;

                    }
                    var ayr = db.Announcement.Where(ax => ax.announcementType == "student" && ax.announcementContent == ans.announcementContent && ax.announcementTitle == ans.announcementTitle && ax.endDate == ans.endDate).FirstOrDefault();
                    if (ayr == null)
                    {
                        db.Announcement.Add(ans);
                        db.SaveChanges();
                        var axr = db.Announcement.Where(ax => ax.announcementType == "student" && ax.announcementContent == ans.announcementContent && ax.announcementTitle == ans.announcementTitle && ax.endDate == ans.endDate).FirstOrDefault();
                        sA.announcementId = axr.announcementID;
                        sA.studentId = avm.studentId;
                        db.studentAnnouncement.Add(sA);
                        db.SaveChanges();

                        HomeroomTeacherMethod htm = new HomeroomTeacherMethod();
                        List<Section> secList = new List<Section>();
                        List<Section> tempSecList = new List<Section>();
                        secList = db.Section.ToList();
                        if (secList != null)
                        {
                            foreach (var k in secList)
                            {
                                if (htm.isInAcademicYear(k.academicYearId))
                                {
                                    tempSecList.Add(k);
                                }
                            }
                            if (tempSecList != null)
                            {
                                foreach (var k in tempSecList)
                                {
                                    int xr = int.Parse(k.sectionName.Substring(0, k.sectionName.Length - 1));
                                    string xr1 = xr.ToString();
                                    var gr = avm2.gradeList.Where(ax => ax.grade == xr).FirstOrDefault();
                                    if (gr == null)
                                    {
                                        gradeViewModel gvm = new gradeViewModel();
                                        gvm.grade = xr;
                                        List<Student> stdList = new List<Student>();
                                        stdList = db.Student.Where(ax => ax.academicYearId == k.academicYearId && ax.sectionName.StartsWith(xr1)).ToList();
                                        gvm.numberOfStudents = stdList.Count();
                                        gvm.numberoFSections = tempSecList.Where(ax => ax.sectionName.StartsWith(xr1)).Count();
                                        gvm.academicYear = k.academicYearId;
                                        avm2.gradeList.Add(gvm);
                                    }
                                }
                            }
                        }
                        List<Announcement> annList = new List<Announcement>();
                        annList = db.Announcement.ToList();
                        if (annList != null)
                        {
                            foreach (var k in annList)
                            {
                                if (DateTime.Compare(DateTime.Now.Date, k.endDate.Date) <= 0)
                                {
                                    avm2.announcementList.Add(k);
                                }
                            }
                        }
                        return View("announcementManagement", avm2);
                    }
                    else
                    {
                        ViewBag.existsMessage = "Announcement Exists";
                        goto x;
                    }
                }
                    else
                    {
                        ViewBag.incorrectFileFormat = "Invalid file format. Please upload pdf file";
                    }
            }
            }
            x:         
            return View(avm);
        }
        [HttpGet]
        public ActionResult updateAnnouncement(string annId,string txtUpdateAnnouncement)
        {
            int annId1 = int.Parse(txtUpdateAnnouncement);
            ApplicationDbContext db = new ApplicationDbContext();
            Announcement an = new Announcement();
            updateAnnouncementViewModel avm = new updateAnnouncementViewModel();
            an = db.Announcement.Where(ax => ax.announcementID == annId1).FirstOrDefault();
            avm.announcementID = an.announcementID;
            avm.announcementTitle = an.announcementTitle;
            avm.announcementContent = an.announcementContent;
            avm.endDate = an.endDate.ToShortDateString();
            
            
            
            return View(avm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult updateAnnouncement(string fileName, string update, HttpPostedFileBase file, updateAnnouncementViewModel avm)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Announcement an = new Announcement();
            avm.gradeList = new List<gradeViewModel>();
            avm.announcementList = new List<Announcement>();
            announcementViewModel avm2 = new announcementViewModel();
            avm2.gradeList = new List<gradeViewModel>();
            avm2.announcementList = new List<Announcement>();
            an = db.Announcement.Where(ax => ax.announcementID == avm.announcementID).FirstOrDefault();
            if (update != null)
            {
                if (ModelState.IsValid==true)
                {
                    if ((file != null && file.ContentType == "application/pdf") || file == null)
                    { 
                    an.announcementContent = avm.announcementContent;
                    an.endDate = Convert.ToDateTime(avm.endDate).Date;
                    an.postDate = DateTime.Now.Date;
                    if (file != null)
                    {
                        int length = file.ContentLength;
                        byte[] upload = new byte[length];
                        file.InputStream.Read(upload, 0, length);
                        an.announcementDocument = upload;
                        an.filName = fileName;
                         

                    }
                    an.updateStatus = 1;
                    an.viewedStatus = 0;
                   /* var ayr = db.Announcement.Where(ax => ax.announcementType == an.announcementType && ax.announcementContent == an.announcementContent && ax.announcementTitle == an.announcementTitle && ax.endDate == an.endDate).FirstOrDefault();
                    if (ayr == null)
                    {*/
                        db.SaveChanges();
                        HomeroomTeacherMethod htm = new HomeroomTeacherMethod();
                        List<Section> secList = new List<Section>();
                        List<Section> tempSecList = new List<Section>();
                        secList = db.Section.ToList();
                        if (secList != null)
                        {
                            foreach (var k in secList)
                            {
                                if (htm.isInAcademicYear(k.academicYearId))
                                {
                                    tempSecList.Add(k);
                                }
                            }
                            if (tempSecList != null)
                            {
                                foreach (var k in tempSecList)
                                {
                                    int xr = int.Parse(k.sectionName.Substring(0, k.sectionName.Length - 1));
                                    string xr1 = xr.ToString();
                                    var gr = avm2.gradeList.Where(ax => ax.grade == xr).FirstOrDefault();
                                    if (gr == null)
                                    {
                                        gradeViewModel gvm = new gradeViewModel();
                                        gvm.grade = xr;
                                        List<Student> stdList = new List<Student>();
                                        stdList = db.Student.Where(ax => ax.academicYearId == k.academicYearId && ax.sectionName.StartsWith(xr1)).ToList();
                                        gvm.numberOfStudents = stdList.Count();
                                        gvm.numberoFSections = tempSecList.Where(ax => ax.sectionName.StartsWith(xr1)).Count();
                                        gvm.academicYear = k.academicYearId;
                                        avm2.gradeList.Add(gvm);
                                    }
                                }
                            }
                        }
                        List<Announcement> annList = new List<Announcement>();
                        annList = db.Announcement.ToList();
                        if (annList != null)
                        {
                            foreach (var k in annList)
                            {
                                if (DateTime.Compare(DateTime.Now.Date, k.endDate.Date) <= 0)
                                {
                                    avm2.announcementList.Add(k);
                                }
                            }
                        }
                        return View("announcementManagement", avm2);
                  //  }
                   


                }
                    else
                    {
                        ViewBag.incorrectFileFormat = "Invalid file format. Please upload pdf file";
                    }

            }
        }
          xo: 
            return View(avm);
        }
        public ActionResult announcementManagement()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            announcementViewModel avm = new announcementViewModel();
            HomeroomTeacherMethod htm = new HomeroomTeacherMethod();
            DateTime tuday = DateTime.Now.Date;
            
            avm.gradeList = new List<gradeViewModel>();
            List<Section> secList = new List<Section>();
            List<Section> tempSecList = new List<Section>();
            avm.announcementList = new List<Announcement>();
            secList = db.Section.ToList();
            if (secList != null)
            {
                foreach(var k in secList)
                {
                    if (htm.isInAcademicYear(k.academicYearId))
                    {
                        tempSecList.Add(k);
                    }
                }
                if (tempSecList != null)
                {
                    foreach(var k in tempSecList)
                    {
                        int xr = int.Parse(k.sectionName.Substring(0, k.sectionName.Length - 1));
                        string xr1 = xr.ToString();
                        var gr = avm.gradeList.Where(ax => ax.grade == xr).FirstOrDefault();
                        if (gr == null)
                        {
                            gradeViewModel gvm = new gradeViewModel();
                            gvm.grade = xr;
                            List<Student> stdList = new List<Student>();
                            stdList = db.Student.Where(ax => ax.academicYearId == k.academicYearId && ax.sectionName.StartsWith(xr1)).ToList();
                            gvm.numberOfStudents = stdList.Count();
                            gvm.numberoFSections = tempSecList.Where(ax => ax.sectionName.StartsWith(xr1)).Count();
                            gvm.academicYear = k.academicYearId;
                            avm.gradeList.Add(gvm);                            
                        }
                    }
                 }
            }
            List<Announcement> annList = new List<Announcement>();
            annList = db.Announcement.ToList();
            if (annList != null)
            {
                foreach(var k in annList)
                {
                    if (DateTime.Compare(DateTime.Now.Date, k.endDate.Date)<=0)
                    {
                        avm.announcementList.Add(k);
                    }
                }
            }
            
            return View(avm);
        }

        [HttpPost ]
        [ValidateAntiForgeryToken]
        public ActionResult announcementManagement(string annId,string delete)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Announcement ann = new Announcement();
            announcementViewModel avm = new announcementViewModel();
            avm.gradeList = new List<gradeViewModel>();
            avm.announcementList = new List<Announcement>();
            studentAnnouncement stda = new studentAnnouncement();
            gradeAnnouncement gra = new gradeAnnouncement();
            List<sectionAnnouncement> sa = new List<sectionAnnouncement>();
            ViewBag.SuccessfulMessage = "";
            int annId1 = int.Parse(annId);
            ann = db.Announcement.Where(ax => ax.announcementID == annId1).FirstOrDefault();
            if (delete != null)
            {
                if (!(Object.ReferenceEquals(ann,null))) { 
                if (ann.announcementType == "student")
                {
                    stda = db.studentAnnouncement.Where(ax => ax.announcementId == ann.announcementID).FirstOrDefault();
                    db.studentAnnouncement.Remove(stda);
                    db.SaveChanges();
                    db.Announcement.Remove(ann);
                    db.SaveChanges();
                    ViewBag.SuccessfulMessage = "Announcement deleted Successfully";
                }
                else if (ann.announcementType == "section")
                {
                    sa = db.sectionAnnouncement.Where(ax => ax.announcementId == ann.announcementID).ToList();
                    foreach (var k in sa)
                    {
                        db.sectionAnnouncement.Remove(k);
                        db.SaveChanges();
                    }
                    db.Announcement.Remove(ann);
                    db.SaveChanges();
                    ViewBag.SuccessfulMessage = "Announcement deleted Successfully";

                }
                else if (ann.announcementType == "all")
                {
                    Announcement ann2 = new Announcement();
                    ann2 = db.Announcement.Where(ax => ax.announcementID == ann.announcementID).FirstOrDefault();
                    db.Announcement.Remove(ann2);
                    db.SaveChanges();
                }
                else
                {
                    gra = db.gradeAnnouncements.Where(ax => ax.announcementId == ann.announcementID).FirstOrDefault();
                    db.gradeAnnouncements.Remove(gra);
                    db.SaveChanges();
                    db.Announcement.Remove(ann);
                    db.SaveChanges();
                    ViewBag.SuccessfulMessage = "Announcement deleted Successfully";
                }
            }
            }
            HomeroomTeacherMethod htm = new HomeroomTeacherMethod();
            List<Section> secList = new List<Section>();
            List<Section> tempSecList = new List<Section>();
            secList = db.Section.ToList();
            if (secList != null)
            {
                foreach (var k in secList)
                {
                    if (htm.isInAcademicYear(k.academicYearId))
                    {
                        tempSecList.Add(k);
                    }
                }
                if (tempSecList != null)
                {
                    foreach (var k in tempSecList)
                    {
                        int xr = int.Parse(k.sectionName.Substring(0, k.sectionName.Length - 1));
                        string xr1 = xr.ToString();
                        var gr = avm.gradeList.Where(ax => ax.grade == xr).FirstOrDefault();
                        if (gr == null)
                        {
                            gradeViewModel gvm = new gradeViewModel();
                            gvm.grade = xr;
                            List<Student> stdList = new List<Student>();
                            stdList = db.Student.Where(ax => ax.academicYearId == k.academicYearId && ax.sectionName.StartsWith(xr1)).ToList();
                            gvm.numberOfStudents = stdList.Count();
                            gvm.numberoFSections = tempSecList.Where(ax => ax.sectionName.StartsWith(xr1)).Count();
                            gvm.academicYear = k.academicYearId;
                            avm.gradeList.Add(gvm);
                        }
                    }
                }
            }
            List<Announcement> annList = new List<Announcement>();
            annList = db.Announcement.ToList();
            if (annList != null)
            {
                foreach (var k in annList)
                {
                    if (DateTime.Compare(DateTime.Now.Date, k.endDate.Date) <= 0)
                    {
                        avm.announcementList.Add(k);
                    }
                }
            }


            return View(avm);
        }
        [HttpGet]
        public ActionResult addFullAnnouncement()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            //sending empty data to the post method of full announcement page
            
            fullAnnouncementViewModel fvm = new fullAnnouncementViewModel();
            return View(fvm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult addFullAnnouncement(string fileName, HttpPostedFileBase file, fullAnnouncementViewModel fvm,string add)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            fvm.gradeList = new List<gradeViewModel>();
            fvm.announcementList = new List<Announcement>();
            Announcement ann = new Announcement();
            announcementViewModel avm2 = new announcementViewModel();
            avm2.gradeList = new List<gradeViewModel>();
            avm2.announcementList = new List<Announcement>();
            if (add != null)
            {
                if (ModelState.IsValid)
                {
                    if ((file != null && file.ContentType == "application/pdf")) { 
                    ann.announcementTitle = fvm.announcementTitle;
                    ann.announcementType = "all";
                    ann.postDate = DateTime.Now.Date;
                    ann.endDate = Convert.ToDateTime(fvm.endDate).Date;
                    ann.announcementContent = fvm.announcementContent;
                    if (file != null)
                    {
                        int length = file.ContentLength;
                        byte[] upload = new byte[length];
                        file.InputStream.Read(upload, 0, length);
                        ann.announcementDocument = upload;
                        ann.filName = fileName;
                    }
                    var ayr = db.Announcement.Where(ax => ax.announcementType == "all" && ax.announcementContent == ann.announcementContent && ax.announcementTitle == ann.announcementTitle && ax.endDate == ann.endDate).FirstOrDefault();
                    if (ayr == null)
                    {
                        db.Announcement.Add(ann);
                        db.SaveChanges();

                        HomeroomTeacherMethod htm = new HomeroomTeacherMethod();
                        List<Section> secList = new List<Section>();
                        List<Section> tempSecList = new List<Section>();
                        secList = db.Section.ToList();
                        if (secList != null)
                        {
                            foreach (var k in secList)
                            {
                                if (htm.isInAcademicYear(k.academicYearId))
                                {
                                    tempSecList.Add(k);
                                }
                            }
                            if (tempSecList != null)
                            {
                                foreach (var k in tempSecList)
                                {
                                    int xr = int.Parse(k.sectionName.Substring(0, k.sectionName.Length - 1));
                                    string xr1 = xr.ToString();
                                    var gr = avm2.gradeList.Where(ax => ax.grade == xr).FirstOrDefault();
                                    if (gr == null)
                                    {
                                        gradeViewModel gvm = new gradeViewModel();
                                        gvm.grade = xr;
                                        List<Student> stdList = new List<Student>();
                                        stdList = db.Student.Where(ax => ax.academicYearId == k.academicYearId && ax.sectionName.StartsWith(xr1)).ToList();
                                        gvm.numberOfStudents = stdList.Count();
                                        gvm.numberoFSections = tempSecList.Where(ax => ax.sectionName.StartsWith(xr1)).Count();
                                        gvm.academicYear = k.academicYearId;
                                        avm2.gradeList.Add(gvm);
                                    }
                                }
                            }
                        }
                        List<Announcement> annList = new List<Announcement>();
                        annList = db.Announcement.ToList();
                        if (annList != null)
                        {
                            foreach (var k in annList)
                            {
                                if (DateTime.Compare(DateTime.Now.Date, k.endDate.Date) <= 0)
                                {
                                    avm2.announcementList.Add(k);
                                }
                            }
                        }
                        return View("announcementManagement", avm2);
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Announcement already exists.";
                        goto xr;
                    }


                }
                    else
                    {
                        ViewBag.incorrectFileFormat = "Invalid file format. Please upload pdf file";
                    }
            }
            }
        xr:

            return View(fvm);
        }





    }
}

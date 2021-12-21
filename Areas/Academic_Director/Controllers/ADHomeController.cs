
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

            //handel viewbag
            ViewBag.search = false;
            ViewBag.upHidden = "hidden";
            ViewBag.disableEmail = " ";

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> RegisterTeacher(RegisterTeacherViewModel registerTeacherViewModel,string register,string search,string update,string edit,string delete,string id)
        {
            //check if their are no errors arise from user input
            if (ModelState.IsValid || (search!=null && ModelState.IsValidField("fullName")) || edit!=null || delete!=null)
            {
                //basic objects
                ApplicationDbContext context = new ApplicationDbContext();
                var userStore = new ApplicationUserStore(context);
                var userManager = new ApplicationUserManager(userStore);
                Collection collection = new Collection();
                AcademicDirector academicDirector = new AcademicDirector();
                Teacher teacher = new Teacher();
                ApplicationUser user = new ApplicationUser();
                RegisterViewModel registerViewModel = new RegisterViewModel();

                //viewbag attributes for UI rendering 
                ViewBag.search = false;
                ViewBag.upHidden = "hidden";
                ViewBag.disableEmail = " ";

                //check if register button is clicked
                if (register != null)
                {
                    //check for a duplicate record
                    if (collection.checkUserExistence(registerTeacherViewModel.email, registerTeacherViewModel.fullName))
                    {
                        //populate RegisterViewModel with the inserted data for registration
                        registerViewModel.fullName = registerTeacherViewModel.fullName;
                        registerViewModel.email = registerTeacherViewModel.email;
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
                            teacher.grade = registerTeacherViewModel.grade;
                            teacher.subject = registerTeacherViewModel.subject;

                            academicDirector.registerTeacher(teacher);

                            //send user credential through email to the new user
                            collection.sendMail(registerViewModel.email, registerViewModel.username, registerViewModel.password);

                            //sucessful message
                            ViewBag.registerStatus = "Registration Completed Successfully";
                        }
                        else
                        {
                            //faliure message due to identity register failure
                            ViewBag.registerStatus = "Registration Failed";
                        }
                    }
                    else
                    {
                        //failure message due to duplicate user
                        ViewBag.duplicate = "Teacher Record Exists with the Same Email Address or Full Name";
                    }
                }
                //check if search button is clicked
                else if (search != null)
                {
                    //remove unwanted error messages
                    ModelState.Remove("email");
                    ModelState.Remove("grade");
                    ModelState.Remove("subject");

                    //viewbag element for UI 
                    ViewBag.search = true;

                    //search teacher using teacher name
                    registerTeacherViewModel.teacherList = context.Teacher.Where(t => t.user.fullName.StartsWith(registerTeacherViewModel.fullName)).ToList();

                    //check if record exist or not
                    if (registerTeacherViewModel.teacherList.Count == 0)
                    {
                        ViewBag.search = false;
                        ViewBag.searchFound = "Record Not Found";
                    }
                }
                //check if update button is clicked
                else if (update != null)
                {
                    //teacher object
                    Teacher teacherUp = new Teacher(1);

                    //assign the new data to teacherUp object
                    var getId = context.Teacher.Where(t => t.user.Email == registerTeacherViewModel.email).FirstOrDefault();
                    teacherUp.teacherId = getId.teacherId;
                    
                    var checkFullName = context.Users.Where(u => u.fullName == registerTeacherViewModel.fullName && u.Id != teacherUp.teacherId).FirstOrDefault();
                    
                    //check if no duplicate name exist
                    if (checkFullName == null)
                    {
                        teacherUp.grade = registerTeacherViewModel.grade;
                        teacherUp.user.fullName = registerTeacherViewModel.fullName;
                        teacherUp.subject = registerTeacherViewModel.subject;

                        //update teacher record 
                        academicDirector.UpdateTeacher(teacherUp);

                        //update successful message
                        ViewBag.updateStatus = "Update Completed Successfully";
                        //ViewBag.upHidden = "hidden";
                        ViewBag.disableEmail = false;
                    }
                    else
                    {
                        //error message for duplicate Full Name
                        ViewBag.fullName = "Full Name Already Taken By Another Account";
                    }
                }
                //check if edit button is clicked
                else if (edit != null)
                {
                    //remove unwanted error messages
                    ModelState.Clear();

                    //check teacher role before editing
                    if (!(userManager.IsInRole(id, "UnitLeader") || userManager.IsInRole(id, "HoomRoom")))
                    {
                        //populate the selected teacher data in to the update form
                        registerTeacherViewModel.teacherList = new List<Teacher>();
                        teacher = context.Teacher.Find(id);
                        registerTeacherViewModel.fullName = teacher.user.fullName;
                        registerTeacherViewModel.subject = teacher.subject;
                        registerTeacherViewModel.grade = teacher.grade;
                        registerTeacherViewModel.email = teacher.user.Email;
                        registerTeacherViewModel.teacherList.Add(teacher);

                        //viewbag elements
                        ViewBag.disableEmail = true;
                        ViewBag.upHidden = " ";
                    }
                    else
                    {
                        //error message for additional role
                        ViewBag.role = "Unable To Edit Because Teacher has Another Role Associated";
                        ViewBag.upHidden = "hidden";
                    }
                    
                }
                //check if delete button is clicked
                else if (delete != null)
                {
                    //viewbag elements
                    ViewBag.search = false;
                    ViewBag.upHidden = "hidden";

                    //check role
                    if (!(userManager.IsInRole(id, "UnitLeader") && userManager.IsInRole(id, "HoomRoom")))
                    {
                        //delete the selected user using teacher id
                        academicDirector.DeleteTeacher(id);
                        string status = await collection.DeleteUser(id);

                        if (status == "successful")
                        {
                            //sucess message
                            ViewBag.deleteStatus = "Deletion Completed Successfully";
                        }
                        else
                        {
                            //failure message
                            ViewBag.deleteStatus = "Deletion Failed";
                        }
                    }
                    else
                    {
                        //error message due to additional role
                        ViewBag.role = "Unable To Delete Because Teacher has Another Role Associated";
                        
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
                    string messageBody = "Registrar Account Username:" + rv.username + "Password=" + rv.password;
                        //   c.sendMail(rv.username, messageBody);
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
        
        public ActionResult manageAcademicYear()
        {
            AcademicYearViewModel academicYearViewModel = new AcademicYearViewModel();
            AcademicDirector ad = new AcademicDirector();
            academicYearViewModel.academicList = new List<AcademicYear>();
            academicYearViewModel= ad.listAcademicYear();
            
            return View(academicYearViewModel);
        }
        [HttpPost]
        public ActionResult manageAcademicYear(AcademicYearViewModel ayvm, string add,string update,string select,string acadYN)
        {
            AcademicYear ay = new AcademicYear();
            ApplicationDbContext db = new ApplicationDbContext();
            AcademicDirector ad = new AcademicDirector();
            AcademicYearViewModel academicYearViewModel = new AcademicYearViewModel();
            academicYearViewModel.academicList = new List<AcademicYear>();
            academicYearViewModel = ad.listAcademicYear();
            ViewBag.Message = " ";
            ViewBag.durationMessage = " ";

            if (ModelState.IsValid || select!=null)
            {
                DateTime tempStart = Convert.ToDateTime(ayvm.yearStartTemp);
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
                if (add != null)
                {
                    AcademicYear ayear = new AcademicYear();
                    ayear = db.AcademicYear.Where(a => a.academicYearName == yearStartOne.ToString("MMMM") + yearStartOne.Year.ToString()).Single();
                    if (ayear != null) { 
                    if (ad.validateDuration(ayvm))
                    {
                        ay.academicYearName = yearStartOne.ToString("MMMM") + yearStartOne.Year.ToString();
                        ay.duration = yearStartOne.ToShortDateString() + "-" + yearEndOne.ToShortDateString();
                        ay.quarterOne = zquarterOneStart.ToShortDateString() + "-" + zquarterOneEnd.ToShortDateString();
                        ay.quarterTwo = zquarterTwoStart.ToShortDateString() + "-" + zquarterTwoEnd.ToShortDateString();
                        ay.quarterThree = zquarterThreeStart.ToShortDateString() + "-" + zquarterThreeEnd.ToShortDateString();
                        ay.quarterFour = zquarterFourStart.ToShortDateString() + "-" + zquarterFourEnd.ToShortDateString();
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

                else if (update != null)
                {

                    string k = tempStart.ToString("MMMM") + tempStart.Year.ToString();
                    ay = db.AcademicYear.Find(tempStart.ToString("MMMM") + tempStart.Year.ToString());
                    string[] splitItems = ay.duration.Split('-');
                    DateTime originalYearStart = Convert.ToDateTime(splitItems[0]);
                    yearStartOne = originalYearStart;
               
                    if (ad.validateDuration(ayvm))
                    {
                        ay.duration = yearStartOne.ToShortDateString() + "-" + yearEndOne.ToShortDateString();
                        ay.quarterOne = zquarterOneStart.ToShortDateString() + "-" + zquarterOneEnd.ToShortDateString();
                        ay.quarterTwo = zquarterTwoStart.ToShortDateString() + "-" + zquarterTwoEnd.ToShortDateString();
                        ay.quarterThree = zquarterThreeStart.ToShortDateString() + "-" + zquarterThreeEnd.ToShortDateString();
                        ay.quarterFour = zquarterFourStart.ToShortDateString() + "-" + zquarterFourEnd.ToShortDateString();
                        db.SaveChanges();
                    }
                }
                else if (select != null)
                {
                    var academicYear = db.AcademicYear.Where(a => a.academicYearName == acadYN).ToList();

                    foreach (var kd in academicYear)
                    {

                        string[] splitItems = kd.duration.Split('-');
                        yearStartOne = Convert.ToDateTime(splitItems[0]);
                        yearEndOne = Convert.ToDateTime(splitItems[1]);
                        Array.Clear(splitItems, 0, splitItems.Length);
                        splitItems = kd.quarterOne.Split('-');
                        zquarterOneStart = Convert.ToDateTime(splitItems[0]);
                        zquarterOneEnd = Convert.ToDateTime(splitItems[1]);
                        Array.Clear(splitItems, 0, splitItems.Length);
                        splitItems = kd.quarterTwo.Split('-');
                        zquarterTwoStart = Convert.ToDateTime(splitItems[0]);
                        zquarterTwoEnd = Convert.ToDateTime(splitItems[1]);
                        Array.Clear(splitItems, 0, splitItems.Length);
                        splitItems = kd.quarterTwo.Split('-');
                        zquarterThreeStart = Convert.ToDateTime(splitItems[0]);
                        zquarterThreeEnd = Convert.ToDateTime(splitItems[1]);
                        Array.Clear(splitItems, 0, splitItems.Length);
                        splitItems = kd.quarterTwo.Split('-');
                        zquarterFourStart = Convert.ToDateTime(splitItems[0]);
                        zquarterFourEnd = Convert.ToDateTime(splitItems[1]);
                    }
                    academicYearViewModel.yearStartTemp = yearStartOne.ToShortDateString();
                    academicYearViewModel.yearStart = yearStartOne.ToShortDateString();
                    academicYearViewModel.yearEnd = yearEndOne.ToShortDateString();
                    academicYearViewModel.quarterOneStart = zquarterOneStart.ToShortDateString();
                    academicYearViewModel.quarterOneEnd = zquarterOneEnd.ToShortDateString();
                    academicYearViewModel.quarterTwoStart = zquarterTwoStart.ToShortDateString();
                    academicYearViewModel.quarterTwoEnd = zquarterTwoEnd.ToShortDateString();
                    academicYearViewModel.quarterThreeStart = zquarterThreeStart.ToShortDateString();
                    academicYearViewModel.quarterThreeEnd = zquarterThreeEnd.ToShortDateString();
                    academicYearViewModel.quarterFourStart = zquarterFourStart.ToShortDateString();
                    academicYearViewModel.quarterFourEnd = zquarterFourEnd.ToShortDateString();



                }
            }
         

           
            
          
            return View(academicYearViewModel);
        }
        public ActionResult unitLeaderManagement()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            AcademicDirector ad = new AcademicDirector();
            RegisterTeacherViewModel rvm = new RegisterTeacherViewModel();
            rvm.teacherList = new List<Teacher>();
            rvm.retrevedTeacherList = new List<Teacher>();

            List<Teacher> temp1 = new List<Teacher>();
            List<Teacher> temp2 = new List<Teacher>();
            temp1 = db.Teacher.ToList();
            foreach(var k in temp1)
            {
                if (ad.IsTeacherorUnitLeader(k.user.Id, "Teacher"))
                {
                    rvm.retrevedTeacherList.Add(k);
                } 

            }
            temp2 = db.Teacher.ToList();
            foreach(var k in temp2)
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
            List<Teacher> temp1 = new List<Teacher>();
            List<Teacher> temp2 = new List<Teacher>();
            List<Teacher> temp3 = new List<Teacher>();
            ViewBag.Message = " ";
            //

            rvm.teacherList = new List<Teacher>();
            rvm.retrevedTeacherList = new List<Teacher>();
            List<Teacher> retrieveAssignment = new List<Teacher>();
            if (selectToAssign != null)
            {
               retrieveAssignment = db.Teacher.Where(a => a.teacherId == teacherID).ToList();
                foreach(var k in retrieveAssignment)
                {
                    rvm.fullName = k.user.fullName;
                    rvm.grade = k.grade;
                    rvm.subject = k.subject;
                }
            }
            else if (assign != null)
            {
                retrieveAssignment = db.Teacher.Where(a => a.grade == rvm.grade && a.subject==rvm.subject && a.user.fullName==rvm.fullName).ToList();
                foreach(var k in retrieveAssignment)
                {
                    teacherID = k.user.Id;
                }
                appUser = userManager.FindById(teacherID);
                var oldRoleId = appUser.Roles.SingleOrDefault().RoleId;
                
                var oldRoleName = db.Roles.SingleOrDefault(r => r.Id == oldRoleId).Name;
                var teacherList = db.Teacher.Where(a => a.grade == a.grade);
                int status = 0;
                foreach(var i in teacherList)
                {
                    if (ad.IsTeacherorUnitLeader(i.teacherId, "UnitLeader")){
                        status = status + 1;
                    }
                }
                if (status == 0) { 
                userManager.RemoveFromRole(appUser.Id, oldRoleName);
                userManager.AddToRole(appUser.Id, "UnitLeader");
                }
                else
                {
                    ViewBag.Message = "Unit leader of the grade already exists.";
                }

            }
            else if (update != null)
            {
                retrieveAssignment = db.Teacher.Where(a => a.grade == rvm.grade && a.subject == rvm.subject && a.user.fullName == rvm.fullName).ToList();
                foreach (var k in retrieveAssignment)
                {
                    teacherID = k.user.Id;
                }
                temp3 = db.Teacher.ToList();
                foreach (var k in temp3)
                {
                    if (ad.IsTeacherorUnitLeader(k.user.Id, "UnitLeader"))
                    {
                        if (k.grade == rvm.grade)
                        {
                            userManager.RemoveFromRole(k.user.Id, "UnitLeader");
                            userManager.AddToRole(k.user.Id, "Teacher");
                            userManager.RemoveFromRole(teacherID, "Teacher");
                            userManager.AddToRole(teacherID, "UnitLeader");
                        }
                       
                    }
                    
                }


            }
            
            temp1 = db.Teacher.ToList();
            foreach (var k in temp1)
            {
                if (ad.IsTeacherorUnitLeader(k.user.Id, "Teacher"))
                {
                    rvm.retrevedTeacherList.Add(k);
                }

            }
            temp2 = db.Teacher.ToList();
            foreach (var k in temp2)
            {
                if (ad.IsTeacherorUnitLeader(k.user.Id, "UnitLeader"))
                {
                    rvm.teacherList.Add(k);
                }
            }
            return View(rvm);
            
        }

        
       


        public ActionResult SectionManagement()
        {
            //Academic Director object
            AcademicDirector academicDirector = new AcademicDirector();

            //viewbag element
            ViewBag.search = false;
            ViewBag.upHidden = "hidden";
            ViewBag.read = false;

            return View(academicDirector.populateFormData());
        }

        [HttpPost]
        public ActionResult SectionManagement(SectionViewModel sectionViewModel,string letter,string teachers,string academicYears,string add, string search, string update, string delete)
        {
            //basic objects
            ApplicationDbContext context = new ApplicationDbContext();
            AcademicDirector academicDirector = new AcademicDirector();
            Section section = new Section();
            Teacher teacher = new Teacher();
            var userStore = new ApplicationUserStore(context);
            var userManager = new ApplicationUserManager(userStore);
            SectionViewModel sectionViewModelExtra = new SectionViewModel();

            //viewbag element used in UI
            ViewBag.search = false;
            ViewBag.upHidden = "hidden";
            ViewBag.read = false;

            //check if Add button is clicked
            if (add != null)
            {
                //check if the record doesn't exist
                sectionViewModelExtra = academicDirector.searchSection(sectionViewModel.grade,letter);
                if (sectionViewModelExtra==null)
                {
                    //get teacher Id and academic year Id 
                    var findTeacher = context.Teacher.Where(t => t.user.fullName == teachers).FirstOrDefault();
                    section.teacherId = findTeacher.teacherId;
                    section.academicYearId = academicYears;

                    //concatenate grade and section letter as a sectionName
                    section.sectionName = sectionViewModel.grade.ToString() + letter;

                    //save section record
                    context.Section.Add(section);
                    context.SaveChanges();

                    //assign HomeRoom role for the selected teacher(remove teacher role)
                    userManager.RemoveFromRole(section.teacherId, "Teacher");
                    userManager.AddToRole(section.teacherId, "HomeRoom");

                    //populate selection list
                    sectionViewModel = academicDirector.populateFormData();

                    //success message
                    ViewBag.add = "Section Created Successfully";
                }
                else
                {
                    //error message
                    ViewBag.add = "The Section Already Exist on Either Selected or Other Active Academic Year";
                }
            }
            //check if search button is clicked
            else if (search != null)
            {
                //remove unwanted errors
                ModelState.Remove("teachers");
                ModelState.Remove("academicYears");

                //search section record
                sectionViewModelExtra = academicDirector.searchSection(sectionViewModel.grade,letter);

                //check if the record exists
                if (sectionViewModelExtra != null)
                {
                    //viewbag attribute for UI usage
                    ViewBag.search = true;
                    ViewBag.upHidden = " ";
                    ViewBag.read= true;

                    //populate other selections in the sectionViewModel
                    sectionViewModel = academicDirector.populateFormData();

                    //include section home room in to the teacher selection
                    foreach(var getTeacher in sectionViewModelExtra.teachers)
                    {
                        sectionViewModel.teachers.Insert(0,getTeacher);
                        ViewBag.teacher = getTeacher;
                    }
                    //get acadamic year of the section record
                    foreach (var getAcademicYear in sectionViewModelExtra.academicYears)
                    {
                        ViewBag.academicYear = getAcademicYear;
                    }
                }
                else
                {
                    //record not found error message
                    ViewBag.found = "Section Not Found";
                    sectionViewModel = academicDirector.populateFormData();

                }
            }
            else if (update != null)
            {
                //check if it exists (also the year)and update
                var sectionRecord = context.Section.Where(s => s.sectionName == sectionViewModel.grade.ToString() + letter && s.academicYearId == academicYears).FirstOrDefault();

                if (sectionRecord != null)
                {
                    //demote role from HoomRoom to Teacher
                    userManager.RemoveFromRole(sectionRecord.teacherId, "HoomRoom");
                    userManager.AddToRole(sectionRecord.teacherId, "Teacher");
                    var newTeacherId = context.Teacher.Where(t => t.user.fullName == teachers).FirstOrDefault();

                    //get section to update using sectionId
                    var updateRecord = context.Section.Find(sectionRecord.sectionId);
                    updateRecord.teacherId = newTeacherId.teacherId;

                    //update section
                    context.SaveChanges();

                    //promote role from Teacher to HoomRoom
                    userManager.RemoveFromRole(newTeacherId.teacherId, "Teacher");
                    userManager.AddToRole(newTeacherId.teacherId, "HomeRoom");

                }

                //update success message 
                ViewBag.update = "Update Completed Successfully";
                ViewBag.read = false;
                ViewBag.upHidden = "hidden";

                //populate form selection options 
                sectionViewModel = academicDirector.populateFormData();

            }
            else if (delete != null)
            {
                //remove unwanted errors
                ModelState.Remove("teachers");
                ModelState.Remove("academicYears");

                //search section record
                sectionViewModelExtra = academicDirector.searchSection(sectionViewModel.grade, letter);

                if (sectionViewModelExtra != null)
                {
                    //check if their are no students enrolled in the section
                    Student student = new Student();
                    var checkStudent = context.Student.Where(s => s.sectionName == sectionViewModelExtra.grade.ToString() + sectionViewModelExtra.letter[0].ToString() && s.academicYearId == sectionViewModelExtra.academicYears[0]).ToList();
                    if (checkStudent.Count == 0)
                    {
                        //delete section
                        var sectionRecord = context.Section.Where(s => s.sectionName == sectionViewModelExtra.grade.ToString() + sectionViewModelExtra.letter[0].ToString() && s.academicYearId == sectionViewModelExtra.academicYears[0]).FirstOrDefault();
                        context.Section.Remove(sectionRecord);
                        context.SaveChanges();

                        //demote role from HoomRoom to Teacher
                        userManager.RemoveFromRole(sectionRecord.teacherId, "HoomRoom");
                        userManager.AddToRole(sectionRecord.teacherId, "Teacher");

                        //successful message
                        ViewBag.delete = "Section Deleted Successfully";
                    }
                    else
                    {
                        ViewBag.delete = "Unable to Delete Section. Students are Enrolled";
                    }
                }
                else
                {
                    //error message
                    ViewBag.delete = "Unable to Delete Section Not Found";
                }
            }

            return View(sectionViewModel);
        }


        public ActionResult StudentManagement()
        {
            //populate the list of section name
            ApplicationDbContext context = new ApplicationDbContext();
            StudentViewModel studentViewModel = new StudentViewModel();
            AcademicDirector academicDirector = new AcademicDirector();
            studentViewModel.sectionName = new List<string>();
            //to get active academic years
            //we need to get the academic year id
            //then if and for loop

            //var academicYears = context.AcademicYear.ToList();

            //foreach (var getActive in academicYears)
            //{
            //    string[] duration = getActive.duration.Split('-');
            //    if (!(DateTime.Compare(DateTime.Now, DateTime.Parse(duration[0])) < 0 || DateTime.Compare(DateTime.Now, DateTime.Parse(duration[1])) > 0))
            //    {
            //        var getSection = context.Section.Where(s => s.academicYearId == getActive.academicYearName).ToList();

            //        foreach(var getSectionName in getSection)
            //        {
            //            studentViewModel.sectionName.Add(getSectionName.sectionName);
            //        }

            //    }
            //}


            studentViewModel.sectionName=academicDirector.populateSection();

            ViewBag.edit = false;
            ViewBag.search = false;

            return View(studentViewModel);
        }
        [HttpPost]
        public ActionResult StudentManagement(StudentViewModel studentViewModel,string sectionName,string register, string search, string update, string delete,string edit,int id)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            AcademicDirector academicDirector = new AcademicDirector();
            Student student = new Student();
            studentViewModel.sectionName = new List<string>();
            ViewBag.edit = false;
            ViewBag.search = false;

            if (register != null)
            {

                //AcademicYear academicYear = new AcademicYear();

                //academicyear
                var academicYears = context.AcademicYear.ToList();
                foreach (var getAcadamicYear in academicYears)
                {
                    string[] duration = getAcadamicYear.duration.Split('-');
                    if (!(DateTime.Compare(DateTime.Now, DateTime.Parse(duration[0])) < 0 || DateTime.Compare(DateTime.Now, DateTime.Parse(duration[1])) > 0))
                    {
                        var sectionRecord = context.Section.Where(s => s.sectionName == sectionName && s.academicYearId == getAcadamicYear.academicYearName).ToList();
                        if (sectionRecord.Count != 0)
                        {
                            student.academicYearId = getAcadamicYear.academicYearName;
                            break;
                        }


                    }
                }

                student.fullName = studentViewModel.fullName;
                student.sectionName = sectionName;

                context.Student.Add(student);
                context.SaveChanges();

                studentViewModel.sectionName = academicDirector.populateSection();

            }
            else if (search != null)
            {

                studentViewModel.student = new List<Student>();
                studentViewModel.student = context.Student.Where(s => s.fullName.StartsWith(studentViewModel.fullName)).ToList();

                studentViewModel.sectionName = academicDirector.populateSection();
                ViewBag.search = true;

            }
            else if (edit != null)
            {
                //populate the data using the id passed
                ViewBag.edit = true;

                student = context.Student.Find(id);
                studentViewModel.Id = student.studentId;
                studentViewModel.fullName = student.fullName;
                studentViewModel.sectionName = new List<string>();

                studentViewModel.sectionName = academicDirector.populateSection();

                var sectionExist = studentViewModel.sectionName.Find(f => f.Equals(student.sectionName));

                if (sectionExist == null)
                {
                    studentViewModel.sectionName.Add(student.sectionName);
                }

                ViewBag.section = student.sectionName;
            }
            else if (update != null)
            {
                //updaate the normal way
                var studentUp = context.Student.Find(studentViewModel.Id);

                studentUp.fullName = studentViewModel.fullName;

                var academicYears = context.AcademicYear.ToList();

                foreach (var getActive in academicYears)
                {
                    string[] duration = getActive.duration.Split('-');
                    if (!(DateTime.Compare(DateTime.Now, DateTime.Parse(duration[0])) < 0 || DateTime.Compare(DateTime.Now, DateTime.Parse(duration[1])) > 0))
                    {
                        var getSection = context.Section.Where(s => s.academicYearId == getActive.academicYearName && s.sectionName==studentUp.sectionName).ToList();

                        if (getSection.Count != 0)
                        {
                            studentUp.sectionName = sectionName;
                            studentUp.academicYearId = getActive.academicYearName;
                            context.SaveChanges();
                        }
                        
                    }
                }

                studentViewModel.sectionName = academicDirector.populateSection();
            }
            else if (delete != null)
            {
                ApplicationDbContext contextExtra = new ApplicationDbContext();

                var parentDelete = contextExtra.Parent.Where(p => p.studentId == id).ToList(); ;
                var studentDelete = context.Student.Find(id);

                if (parentDelete.Count != 0)
                {
                    foreach (var getParent in parentDelete)
                    {
                        contextExtra.Parent.Remove(getParent);
                        contextExtra.SaveChanges();
                    }
                }
     
                context.Student.Remove(studentDelete);
                context.SaveChanges();

                studentViewModel.sectionName = academicDirector.populateSection();

            }
            return View(studentViewModel);
        }
        public ActionResult parentManagement()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            parentViewModel parent = new parentViewModel();
            parent.parentList = new List<Models.Parent>();
            parent.parentList = db.Parent.ToList();
            parent.studentList = new List<Student>();
            
            return View(parent);
        }
        [HttpPost]
        public async Task<ActionResult> parentManagement(string add,string delete,string select,string search,parentViewModel pv,string id,string pid)
        {
            ViewBag.Message = " ";
            ApplicationDbContext db = new ApplicationDbContext();
            ApplicationUser au = new ApplicationUser();
            pv.studentList = new List<Student>();
            pv.parentList = new List<Models.Parent>();
            Student st = new Student();
            Models.Parent p = new Models.Parent();
            Collection c = new Collection();
            RegisterViewModel rv = new RegisterViewModel();
            Models.Parent parent = new Models.Parent();
            var userStore = new ApplicationUserStore(db);
            var userManager = new ApplicationUserManager(userStore);

            if (ModelState.IsValid || delete!=null || select!=null || search!=null) { 
            if (search != null)
            {
                    if (pv.studentFullName != null)
                    {
                        ViewBag.startStudent = true;
                        //searching a student information using a student name
                        pv.studentList = db.Student.Where(a => a.fullName.StartsWith(pv.studentFullName)).ToList();
                    }
                    else
                    {
                        ViewBag.startStudent = false;
                    }
                
                    if (pv.fullName != null)
                    {
                        ViewBag.startParent = true;
                        pv.parentList = db.Parent.Where(a => a.user.fullName.StartsWith(pv.fullName)).ToList();
                    }
                    else
                    {
                        ViewBag.startParent = false;
                    }
                }
            else if (select != null)
            {
                int translatedID = int.Parse(id);
                st = db.Student.Where(a => a.studentId == translatedID).Single();
                pv.studentFullName = st.fullName;
                pv.studentId = translatedID;
                
            }
            else if (add != null)
            {
                    //adding the parent information to registerViewModel rv  abd adding to identity user table
                    List<Models.Parent> l = new List<Models.Parent>();
                    l = db.Parent.Where(a => a.studentId == pv.studentId).ToList();
                   
                    if (l.Count < 2)
                    {
                        rv.email = pv.email;
                        rv.fullName = pv.fullName;
                        rv.username = c.generateUserName();
                        rv.password = c.generatePassword();

                        string ide = c.RegisterUser(rv, "Parent");

                        if (ide != null)
                        {
                            //adding parent to the parent table 
                            parent.parentId = ide;
                            parent.studentId = pv.studentId;
                            db.Parent.Add(parent);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        ViewBag.Message="More than two parents cannot be added.";
                    }
                    
                              
               
            }
            else if (delete != null)
            {

                //deleting the parent information from the identity user and from the parent table.
                p = db.Parent.Find(pid);
                db.Parent.Remove(p);
                db.SaveChanges();
                
                string status = await c.DeleteUser(pid);
               
            }
            }
            return View(pv);
        }


    }
}

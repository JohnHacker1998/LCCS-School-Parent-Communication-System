
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
                    if (ad.validateDuration(ayvm))
                    {
                        ay.academicYearName = yearStartOne.ToString("MMMM") + yearEndOne.Year.ToString();
                        ay.duration = yearStartOne.ToShortDateString() + "-" + yearEndOne.ToShortDateString();
                        ay.quarterOne = zquarterOneStart.ToShortDateString() + "-" + zquarterOneEnd.ToShortDateString();
                        ay.quarterTwo = zquarterTwoStart.ToShortDateString() + "-" + zquarterTwoEnd.ToShortDateString();
                        ay.quarterThree = zquarterThreeStart.ToShortDateString() + "-" + zquarterThreeEnd.ToShortDateString();
                        ay.quarterFour = zquarterFourStart.ToShortDateString() + "-" + zquarterFourEnd.ToShortDateString();
                        db.AcademicYear.Add(ay);
                        db.SaveChanges();
                    }

                }

                else if (update != null)
                {

                    string k = tempStart.ToString("MMMM") + tempStart.Year.ToString();
                    ay = db.AcademicYear.Find(tempStart.ToString("MMMM") + tempStart.Year.ToString());
                    string[] splitItems = ay.duration.Split('-');
                    DateTime originalYearStart = Convert.ToDateTime(splitItems[0]);
                    yearStartOne = originalYearStart;
                    int x = 0;
                    if (ad.validateDuration(ayvm))
                    {
                        int y = 0;
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
            List<Teacher> temp4 = new List<Teacher>();

            // RegisterTeacherViewModel rvm = new RegisterTeacherViewModel();
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
                userManager.RemoveFromRole(appUser.Id, oldRoleName);
                userManager.AddToRole(appUser.Id, "UnitLeader");



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
            AcademicDirector academicDirector = new AcademicDirector();
            ViewBag.search = false;
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

            ViewBag.search = false;

            //check if Add button is clicked
            if (add != null)
            {
                //check if the record doesn't exist
                sectionViewModelExtra = academicDirector.searchSection(sectionViewModel.grade,letter);
                if (sectionViewModelExtra==null)
                {
                    //get teacher Id and academic year Id 
                    var findTeacher = context.Teacher.Where(t => t.user.fullName == teachers);
                    //var findAcadamicYear = context.AcademicYear.Where(a => a.academicYearName == academicYears);

                    //get teacherId
                    foreach (var getTeacherId in findTeacher)
                    {
                        section.teacherId = getTeacherId.teacherId;
                    }

                    //get Academic Year Id

                    section.academicYearId = academicYears;
                    //foreach (var getAcadamicYearName in findAcadamicYear)
                    //{
                    //    section.academicYearId = getAcadamicYearName.academicYearName;
                    //}

                    //concatenate grade and section letter as a sectionName
                    section.sectionName = sectionViewModel.grade.ToString() + letter;

                    //save section record
                    context.Section.Add(section);
                    context.SaveChanges();

                    //assign HomeRoom role for the selected teacher(remove teacher role)
                    userManager.RemoveFromRole(section.teacherId, "Teacher");
                    userManager.AddToRole(section.teacherId, "HomeRoom");

                    sectionViewModel = academicDirector.populateFormData();
                }
            }
            //check if search button is clicked
            else if (search != null)
            {
                ViewBag.search = true;
                //search section record
                sectionViewModelExtra = academicDirector.searchSection(sectionViewModel.grade,letter);

                //check if the record exists
                if (sectionViewModelExtra != null)
                {
                    ViewBag.found = true;
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
                    ViewBag.found = false;
                    sectionViewModel = academicDirector.populateFormData();

                }

                //if not found populate the list views 
                //if exist add the data firdt and add the other list
            }
            else if (update != null)
            {
                //check if it exists (also the year)and update
                var sectionRecord = context.Section.Where(s => s.sectionName == sectionViewModel.grade.ToString() + letter && s.academicYearId == academicYears).ToList();
                
                if (sectionRecord != null)
                {
                    foreach (var data in sectionRecord)
                    {
                        
                        userManager.RemoveFromRole(data.teacherId, "HoomRoom");
                        userManager.AddToRole(data.teacherId, "Teacher");
                        var newTeacherId = context.Teacher.Where(t => t.user.fullName == teachers).ToList();

                        foreach(var getId in newTeacherId)
                        {
                            var updateRecord = context.Section.Find(data.sectionId);
                            updateRecord.teacherId = getId.teacherId;

                            context.SaveChanges();
                            userManager.RemoveFromRole(getId.teacherId, "Teacher");
                            userManager.AddToRole(getId.teacherId, "HomeRoom");
                        }
                        
                    }
                }

                
            }
            else if (delete != null)
            {
                //delete if it exists
                var sectionRecord = context.Section.Where(s => s.sectionName == sectionViewModel.grade.ToString() + sectionViewModel.letter && s.academicYearId == academicYears.ToString());

                if (sectionRecord != null)
                {

                    foreach(var deleteSectionId in sectionRecord)
                    {
                        var deleteSection = context.Section.Find(deleteSectionId.sectionId);
                        context.Section.Remove(deleteSection);
                        context.SaveChanges();
                    }
                    
                }

            }


            return View(sectionViewModel);
        }


        public ActionResult StudentManagement()
        {
            return View();
        }
        [HttpPost]
        public ActionResult StudentManagement(int id)
        {
            return View();
        }


    }
}

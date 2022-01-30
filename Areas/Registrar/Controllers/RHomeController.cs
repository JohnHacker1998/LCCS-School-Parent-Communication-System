using LCCS_School_Parent_Communication_System.Additional_Class;
using LCCS_School_Parent_Communication_System.Identity;
using LCCS_School_Parent_Communication_System.Models;
using LCCS_School_Parent_Communication_System.viewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LCCS_School_Parent_Communication_System.Areas.Registrar.Controllers
{
    [Authorize(Roles = "Registrar")]

    public class RHomeController : Controller
    {
        // GET: Registrar/RHome
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult parentManagement()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            parentViewModel parent = new parentViewModel();
            //instantiating the parent and student list, and populating parents list into parentList for the view.
            parent.parentList = new List<Models.Parent>();
            parent.parentList = db.Parent.ToList();
            parent.studentList = new List<Student>();

            return View(parent);
        }
        [HttpPost]
        public async Task<ActionResult> parentManagement(string add, string delete, string select, string search, parentViewModel pv, string id, string pid)
        {

            ViewBag.Message = " ";
            ViewBag.existenceMessage = " ";
            ApplicationDbContext db = new ApplicationDbContext();
            ApplicationUser au = new ApplicationUser();
            pv.studentList = new List<Student>();
            pv.parentList = new List<Models.Parent>();
            Student st = new Student();
            Models.Parent p = new Models.Parent();
            Collection c = new Collection();
            RegistrarMethod rm = new RegistrarMethod();
            RegisterViewModel rv = new RegisterViewModel();
            Models.Parent parent = new Models.Parent();
            var userStore = new ApplicationUserStore(db);
            var userManager = new ApplicationUserManager(userStore);
            var regex = new Regex(@"[A-Z][a-z]{2,}");


            if (ModelState.IsValid || delete != null || select != null || search != null)
            {
                //if search is clicked
                if (search != null)
                {
                    ModelState.Clear();
                    //if studentFullName returns with value
                    if (pv.studentFullName != null || regex.IsMatch(pv.studentFullName))
                    {
                        //searching the student from the student table and enabling the view to display the student list of the student using viewbag.
                        ViewBag.startStudent = true;
                        //searching a student information using a student name
                        //displaying the student list which starts with the value provieded from the student full name
                        pv.studentList = db.Student.Where(a => a.fullName.StartsWith(pv.studentFullName)).ToList();
                    }
                    else
                    {
                        //ifnot, not showing the list of the student.
                        ViewBag.startStudent = false;
                    }
                    //if fullname is only filled and search is selected.
                    if (pv.fullName != null && regex.IsMatch(pv.fullName))
                    {
                        //display the list of parents eligible to be related with the student
                        ViewBag.startParent = true;
                        //searching the list of parents from the parent table which start with the fullname provided
                        pv.parentList = db.Parent.Where(a => a.user.fullName.StartsWith(pv.fullName)).ToList();
                    }
                    else
                    {
                        ViewBag.startParent = false;
                    }
                }
                //if select is clicked
                else if (select != null)
                {
                    ModelState.Clear();
                    //searching for a student with the specific student id to fill the student name field.
                    int translatedID = int.Parse(id);
                    st = db.Student.Where(a => a.studentId == translatedID).FirstOrDefault();
                    pv.studentFullName = st.fullName;
                    pv.studentId = translatedID;

                }
                //if add is clicked
                else if (add != null)
                {
                    //adding the parent information to registerViewModel rv  abd adding to identity user table
                    List<Models.Parent> l = new List<Models.Parent>();
                    //searching for a parent with the same student id
                    l = db.Parent.Where(a => a.studentId == pv.studentId).ToList();
                    //if there are is 1 or no parents related to the student, it is allowed to relate the student with a parent,
                    if (l.Count < 2)
                    {
                        //adding the parent information into the viewmodel of the register user 
                        //checking the a user exists with the same username or fullname as of the being registered parent
                        if (c.checkUserExistence(pv.email, pv.fullName))
                        {
                            //if user exists, adding the parent information into the identity user table and parent table.
                            rv.email = pv.email;
                            rv.fullName = pv.fullName;
                            rv.username = c.generateUserName();
                            rv.password = c.generatePassword();
                            rv.phoneNumber = pv.phoneNumber;
                            

                            string ide = rm.RegisterParent(rv, "Parent");

                            if (ide != null)
                            {
                                //adding parent to the parent table 
                                parent.parentId = ide;
                                parent.studentId = pv.studentId;
                                db.Parent.Add(parent);
                                db.SaveChanges();
                                c.sendMail(rv.email, rv.username, rv.password);
                            }
                        }
                        else
                        {
                            ViewBag.existenceMessage = "User already exists.";
                        }

                    }
                    else
                    {
                        ViewBag.Message = "More than two parents cannot be added.";
                    }



                }
                //if delete is clicked
                else if (delete != null)
                {
                    ModelState.Clear();
                    //deleting the parent information from the identity user and from the parent table.
                    p = db.Parent.Find(pid);
                    db.Parent.Remove(p);
                    db.SaveChanges();

                    string status = await c.DeleteUser(pid);

                }
            }
            return View(pv);

        }

        public ActionResult RegisterStudent()
        {
            StudentViewModel studentViewModel = new StudentViewModel();
            studentViewModel.sectionName = new List<string>();
            RegistrarMethod registrarMethod = new RegistrarMethod();

            studentViewModel.sectionName = registrarMethod.populateSection();
            return PartialView("RegisterStudent",studentViewModel);
        }

        [HttpPost]
        public ActionResult RegisterStudent(StudentViewModel studentViewModel,string sectionName)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            Student student = new Student();
            RegistrarMethod registrarMethod = new RegistrarMethod();

            var studentExist = context.Student.Where(s => s.fullName == studentViewModel.fullName).FirstOrDefault();

            if (studentExist == null)
            {
                //get selected section academic year
                var academicYears = context.AcademicYear.ToList();
                foreach (var getAcadamicYear in academicYears)
                {
                    if (!(DateTime.Compare(DateTime.Now, getAcadamicYear.durationStart) < 0 || DateTime.Compare(DateTime.Now, getAcadamicYear.durationEnd) > 0))
                    {
                        var sectionRecord = context.Section.Where(s => s.sectionName == sectionName && s.academicYearId == getAcadamicYear.academicYearName).ToList();
                        if (sectionRecord.Count != 0)
                        {
                            //populate student object
                            student.academicYearId = getAcadamicYear.academicYearName;
                            break;
                        }
                    }
                }

                student.fullName = studentViewModel.fullName;
                student.sectionName = sectionName;

                //register Student
                context.Student.Add(student);
                int success=context.SaveChanges();

                if (success > 0)
                {
                    //success message
                    ViewBag.complete = "Student Registered Successfully";
                }
                else
                {
                    ViewBag.error = "Student Registration Failed!!";
                }

                
            }
            else
            {
                //error message
                ViewBag.error = "Student Already Exist";
            }

            studentViewModel.sectionName = registrarMethod.populateSection();

            return PartialView("RegisterStudent",studentViewModel);
        }

        public ActionResult EditStudent(string id)
        {
            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            RegistrarMethod registrarMethod = new RegistrarMethod();
            Student student = new Student();
            StudentViewModel studentViewModel = new StudentViewModel();
            studentViewModel.sectionName = new List<string>();

            int Id = int.Parse(id);

            //populate the data using the id passed
            student = context.Student.Find(Id);
            studentViewModel.Id = student.studentId;
            studentViewModel.fullName = student.fullName;
            

            studentViewModel.sectionName = registrarMethod.populateSection();

            var sectionExist = studentViewModel.sectionName.Find(f => f.Equals(student.sectionName));

            if (sectionExist == null)
            {
                studentViewModel.sectionName.Add(student.sectionName);
            }

            ViewBag.section = student.sectionName;

            return PartialView("EditStudent",studentViewModel);
        }

        [HttpPost]
        public ActionResult EditStudent(StudentViewModel studentViewModel,string sectionName)
        {

            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            RegistrarMethod registrarMethod = new RegistrarMethod();

            //find student by using id
            var studentUp = context.Student.Find(studentViewModel.Id);

            //check if the new name is unique
            var name = context.Student.Where(s => s.fullName == studentUp.fullName && s.studentId != studentUp.studentId).FirstOrDefault();
            if (name == null)
            {
                studentUp.fullName = studentViewModel.fullName;

                var academicYears = context.AcademicYear.ToList();
                foreach (var getActive in academicYears)
                {
                    //get the active academic years
                    if (!(DateTime.Compare(DateTime.Now, getActive.durationStart) < 0 || DateTime.Compare(DateTime.Now, getActive.durationEnd) > 0))
                    {
                        var getSection = context.Section.Where(s => s.academicYearId == getActive.academicYearName && s.sectionName == sectionName).FirstOrDefault();

                        if (getSection != null)
                        {
                            studentUp.sectionName = sectionName;
                            studentUp.academicYearId = getActive.academicYearName;

                            context.Entry(studentUp).State = EntityState.Modified;
                            int success = context.SaveChanges();

                            if (success > 0)
                            {
                                //success message
                                ViewBag.complete = "Student Updated Successfully";
                            }
                            else
                            {
                                //error message
                                ViewBag.error = "Update Failed!!";
                            }

                        }
                        else
                        {
                            //error message
                            ViewBag.error = "Section is Not Active";
                        }

                    }
                }
            }
            else
            {
                //error message
                ViewBag.error = "Student with the Given Name Already Exist";
            }

            studentViewModel.sectionName = registrarMethod.populateSection();

            var student = context.Student.Find(studentViewModel.Id);

            var sectionExist = studentViewModel.sectionName.Find(f => f.Equals(student.sectionName));

            if (sectionExist == null)
            {
                studentViewModel.sectionName.Add(student.sectionName);
            }

            ViewBag.section = student.sectionName;

            studentViewModel.Id = studentViewModel.Id;

            return PartialView("EditStudent",studentViewModel);
        }

        public ActionResult DeleteStudent(string id)
        {
            StudentViewModel studentViewModel = new StudentViewModel();
            studentViewModel.Id = int.Parse(id);

            ViewBag.message = "Are You Sure Do You Want to Delete this Student and Associated Parent?";

            return PartialView("DeleteStudent",studentViewModel);
        }

        [HttpPost]
        public ActionResult DeleteStudent(StudentViewModel studentViewModel)
        {

            //context object
            ApplicationDbContext contextExtra = new ApplicationDbContext();
            ApplicationDbContext context = new ApplicationDbContext();

            //search for student and associated parents
            var parentDelete = contextExtra.Parent.Where(p => p.studentId == studentViewModel.Id).ToList(); ;
            var studentDelete = context.Student.Find(studentViewModel.Id);

            //delete associated parents
            if (parentDelete.Count != 0)
            {
                foreach (var getParent in parentDelete)
                {
                    contextExtra.Parent.Remove(getParent);
                    contextExtra.SaveChanges();
                }
            }

            //delete student record
            context.Student.Remove(studentDelete);
            int success=context.SaveChanges();

            if (success > 0)
            {
                //delete success message
                ViewBag.complete = "Delete Completed Successfully";
            }
            else
            {
                //error message
                ViewBag.posterror = "Deletion Failed!!";
            }

            return PartialView("DeleteStudent");
        }

        public ActionResult StudentManagement()
        {
            //basic objects
            ApplicationDbContext context = new ApplicationDbContext();
            StudentViewModel studentViewModel = new StudentViewModel();
            ////RegistrarMethod registrarMethod = new RegistrarMethod();
            studentViewModel.student = new List<Student>();
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

            //populate available sections
            ////studentViewModel.sectionName = registrarMethod.populateSection();
            studentViewModel.student = context.Student.ToList();

            //viewbag elements used in the UI 
            //ViewBag.edit = false;
            //ViewBag.search = false;
            //ViewBag.update = "hidden";

            return View(studentViewModel);
        }
        //[HttpPost]
        //public ActionResult StudentManagement(StudentViewModel studentViewModel, string sectionName, string register, string search, string update, string delete, string edit, int id)
        //{
        //    //basic objects
        //    ApplicationDbContext context = new ApplicationDbContext();
        //    RegistrarMethod registrarMethod = new RegistrarMethod();
        //    Student student = new Student();
        //    studentViewModel.sectionName = new List<string>();

        //    //viewbag elements for UI
        //    ViewBag.edit = false;
        //    ViewBag.search = false;
        //    ViewBag.update = "hidden";

        //    if (ModelState.IsValid || (search != null && ModelState.IsValidField("fullName")) || edit != null || delete != null)
        //    {
        //        if (register != null)
        //        {
        //            //check if student already exist(duplicate)
        //            var studentExist = context.Student.Where(s => s.fullName == studentViewModel.fullName).FirstOrDefault();

        //            if (studentExist == null)
        //            {
        //                //get selected section academic year
        //                var academicYears = context.AcademicYear.ToList();
        //                foreach (var getAcadamicYear in academicYears)
        //                {
        //                    if (!(DateTime.Compare(DateTime.Now, getAcadamicYear.durationStart) < 0 || DateTime.Compare(DateTime.Now, getAcadamicYear.durationEnd) > 0))
        //                    {
        //                        var sectionRecord = context.Section.Where(s => s.sectionName == sectionName && s.academicYearId == getAcadamicYear.academicYearName).ToList();
        //                        if (sectionRecord.Count != 0)
        //                        {
        //                            //populate student object
        //                            student.academicYearId = getAcadamicYear.academicYearName;
        //                            break;
        //                        }
        //                    }
        //                }

        //                student.fullName = studentViewModel.fullName;
        //                student.sectionName = sectionName;

        //                //register Student
        //                context.Student.Add(student);
        //                context.SaveChanges();

        //                //success message
        //                ViewBag.add = "Student Registered Successfully";
        //            }
        //            else
        //            {
        //                //error message
        //                ViewBag.add = "Student Already Exist";
        //            }

        //            studentViewModel.sectionName = registrarMethod.populateSection();

        //        }
        //        else if (search != null)
        //        {
        //            //search student
        //            studentViewModel.student = new List<Student>();
        //            studentViewModel.student = context.Student.Where(s => s.fullName.StartsWith(studentViewModel.fullName)).ToList();

        //            //populate section list
        //            studentViewModel.sectionName = registrarMethod.populateSection();

        //            if (studentViewModel.student.Count != 0)
        //            {
        //                //viewbag element for UI purpose
        //                ViewBag.search = true;
        //            }
        //            else
        //            {
        //                //error message
        //                ViewBag.found = "Record Not Found";
        //            }
        //            int x = 0;

        //        }
        //        else if (edit != null)
        //        {
        //            //viewbag elements used for UI
        //            ViewBag.edit = true;
        //            ViewBag.update = " ";

        //            //populate the data using the id passed
        //            student = context.Student.Find(id);
        //            studentViewModel.Id = student.studentId;
        //            studentViewModel.fullName = student.fullName;
        //            studentViewModel.sectionName = new List<string>();

        //            studentViewModel.sectionName = registrarMethod.populateSection();

        //            var sectionExist = studentViewModel.sectionName.Find(f => f.Equals(student.sectionName));

        //            if (sectionExist == null)
        //            {
        //                studentViewModel.sectionName.Add(student.sectionName);
        //            }

        //            ViewBag.section = student.sectionName;
        //        }
        //        else if (update != null)
        //        {
        //            //find student by using id
        //            var studentUp = context.Student.Find(studentViewModel.Id);

        //            //check if the new name is unique
        //            var name = context.Student.Where(s => s.fullName == studentUp.fullName && s.studentId != studentUp.studentId).FirstOrDefault();
        //            if (name == null)
        //            {
        //                studentUp.fullName = studentViewModel.fullName;

        //                var academicYears = context.AcademicYear.ToList();
        //                foreach (var getActive in academicYears)
        //                {
        //                    //get the active academic years
        //                    if (!(DateTime.Compare(DateTime.Now, getActive.durationStart) < 0 || DateTime.Compare(DateTime.Now, getActive.durationEnd) > 0))
        //                    {
        //                        var getSection = context.Section.Where(s => s.academicYearId == getActive.academicYearName && s.sectionName == sectionName).FirstOrDefault();

        //                        if (getSection != null)
        //                        {
        //                            studentUp.sectionName = sectionName;
        //                            studentUp.academicYearId = getActive.academicYearName;

        //                            int success=context.SaveChanges();

        //                            if (success>0)
        //                            {
        //                                //success message
        //                                ViewBag.complete = "Student Updated Successfully";
        //                            }
        //                            else
        //                            {
        //                                //error message
        //                                ViewBag.error = "Update Failed!!";
        //                            }
                                    
        //                        }
        //                        else
        //                        {
        //                            //error message
        //                            ViewBag.error = "Section is Not Active";
        //                        }

        //                    }
        //                }
        //            }
        //            else
        //            {
        //                //error message
        //                ViewBag.error = "Student with the Given Name Already Exist";
        //            }

        //            studentViewModel.sectionName = registrarMethod.populateSection();
        //            ViewBag.update = "hidden";
        //        }
        //        else if (delete != null)
        //        {
        //            //context object
        //            ApplicationDbContext contextExtra = new ApplicationDbContext();

        //            //search for student and associated parents
        //            var parentDelete = contextExtra.Parent.Where(p => p.studentId == id).ToList(); ;
        //            var studentDelete = context.Student.Find(id);

        //            //delete associated parents
        //            if (parentDelete.Count != 0)
        //            {
        //                foreach (var getParent in parentDelete)
        //                {
        //                    contextExtra.Parent.Remove(getParent);
        //                    contextExtra.SaveChanges();
        //                }
        //            }

        //            //delete student record
        //            context.Student.Remove(studentDelete);
        //            context.SaveChanges();

        //            //delete success message
        //            ViewBag.delete = "Delete Completed Successfully";

        //            studentViewModel.sectionName = registrarMethod.populateSection();

        //        }
        //    }
            
        //    return View(studentViewModel);
        //}

    }
}
using LCCS_School_Parent_Communication_System.Additional_Class;
using LCCS_School_Parent_Communication_System.Identity;
using LCCS_School_Parent_Communication_System.Models;
using LCCS_School_Parent_Communication_System.viewModels;
using System;
using System.Collections.Generic;
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
            RegisterViewModel rv = new RegisterViewModel();
            Models.Parent parent = new Models.Parent();
            var userStore = new ApplicationUserStore(db);
            var userManager = new ApplicationUserManager(userStore);
            var regex = new Regex(@"([A-Z]{1}[a-z]{2,} ){2}([A-Z]{1}[a-z]{2,})");


            if (ModelState.IsValid || delete != null || select != null || search != null)
            {
                //if search is clicked
                if (search != null)
                {
                    ModelState.Clear();
                    //if studentFullName returns with value
                    if (pv.studentFullName != null && regex.IsMatch(pv.studentFullName))
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

                            string ide = c.RegisterUser(rv, "Parent");

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
    }
}
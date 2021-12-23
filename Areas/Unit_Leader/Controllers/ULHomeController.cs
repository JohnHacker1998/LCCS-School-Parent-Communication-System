using LCCS_School_Parent_Communication_System.Additional_Class;
using LCCS_School_Parent_Communication_System.Identity;
using LCCS_School_Parent_Communication_System.Models;
using LCCS_School_Parent_Communication_System.viewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LCCS_School_Parent_Communication_System.Areas.Unit_Leader.Controllers
{
    [Authorize(Roles = "UnitLeader")]

    public class ULHomeController : Controller
    {
        // GET: Unit_Leader/ULHome
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LateComerManagement()
        {
            //viewbag element used for UI
            ViewBag.search = false;
            return View();
        }
        [HttpPost]
        public ActionResult LateComerManagement(LateComerViewModel lateComerViewModel,string id,string late,string search)
        {
            //basic objects
            ApplicationDbContext context = new ApplicationDbContext();
            Suspension suspension = new Suspension();
            lateComerViewModel.students = new List<Student>();
            LateComer lateComer = new LateComer();
            Collection collection = new Collection();
            //Teacher teacher = new Teacher();

            //viewbag element used for UI
            ViewBag.search = false;
            if (ModelState.IsValid || late!=null)
            {
                //get unitleader info
                string tId = User.Identity.GetUserId().ToString();
                var teacher = context.Teacher.Where(t => t.teacherId == tId).FirstOrDefault();

                //check search button is clicked
                if (search != null)
                {
                    
                    //get all academic years
                    var academicYears = context.AcademicYear.ToList();
                    foreach (var getActive in academicYears)
                    {
                        //get start and end dates to check if today is in the middle
                        string[] duration = getActive.duration.Split('-');
                        if (!(DateTime.Compare(DateTime.Now, DateTime.Parse(duration[0])) < 0 || DateTime.Compare(DateTime.Now, DateTime.Parse(duration[1])) > 0))
                        {
                            //search student by student name in active academic years
                            lateComerViewModel.students = context.Student.Where(s => s.fullName.StartsWith(lateComerViewModel.studentName) && s.academicYearId == getActive.academicYearName && s.sectionName.StartsWith(teacher.grade.ToString())).ToList();
                        }
                    }

                    if (lateComerViewModel.students.Count != 0)
                    {
                        //check the student is suspended or not
                        foreach (var checkStudent in lateComerViewModel.students)
                        {
                            var studentSuspended = context.Suspension.Where(s => s.studentId == checkStudent.studentId).FirstOrDefault();
                            if (studentSuspended != null)
                            {
                                //check if the suspention time elapsed
                                if (DateTime.Compare(studentSuspended.endDate, DateTime.Now) < 0)
                                {
                                    //remove student from suspention table
                                    context.Suspension.Remove(studentSuspended);
                                }
                                else
                                {
                                    //remove student from search list
                                    lateComerViewModel.students.Remove(checkStudent);
                                }
                            }
                        }

                        //viewbag element
                        ViewBag.search = true;
                    }
                    else
                    {
                        //error message
                        ViewBag.found = "Record Not Found";
                    }
                }
                //check late button is clicked
                else if (late != null)
                {
                    //remove errors
                    ModelState.Remove("studentName");

                    int count = 0;
                    int ID = Int32.Parse(id);

                    //get how mach time the student is late and increment it by 1
                    var lateRecord = context.LateComer.Where(l => l.studentId == ID).ToList();
                    if (lateRecord.Count != 0)
                    {
                        //get the late days count
                        foreach (var getCount in lateRecord)
                        {
                            if (count < getCount.dayCount)
                            {
                                count = getCount.dayCount;
                            }
                        }
                    }

                    if (collection.currentQuarter(ID) == " ")
                    {
                        //error message
                        ViewBag.error = "Time is not Bounded in a Quarter";
                    }
                    else
                    {
                        //populate latecomer object
                        lateComer.academicPeriod = collection.currentQuarter(ID);
                        lateComer.dayCount = count + 1;
                        lateComer.lateDate = DateTime.Now;
                        lateComer.studentId = ID;

                        //save latecomer record
                        context.LateComer.Add(lateComer);
                        context.SaveChanges();

                        //send warning message if the counter reaches 3
                        if (lateComer.dayCount == 3 || lateComer.dayCount == 7 || lateComer.dayCount == 11)
                        {
                            //warning object
                            Warning warning = new Warning();

                            //populate warning object
                            warning.studentId = ID;
                            warning.WarningReadStatus = "No";
                            warning.grade = teacher.grade;
                            warning.warningType = "LateComer";

                            //save warning
                            context.Warning.Add(warning);
                            context.SaveChanges();
                        }
                        else if (lateComer.dayCount == 4 || lateComer.dayCount == 8 || lateComer.dayCount == 12)
                        {
                            //suspend student for two days
                            suspension.studentId=ID;
                            suspension.startDate = DateTime.Now.AddDays(1);
                            suspension.endDate = DateTime.Now.AddDays(2);

                            //save suspension record
                            context.Suspension.Add(suspension);
                            context.SaveChanges();
                        }

                        //success message
                        ViewBag.message = "LateComer Recorded Successfully";
                    }
                }
            }
            
            return View(lateComerViewModel);
        }

        public ActionResult WarningManagement()
        {
            //check if the attendance recourd count reach 3 and 5
            //check if the previous day attendance record is present
            //check their grades

            //basic objects
            ApplicationDbContext context = new ApplicationDbContext();
            Warning warning = new Warning();
            WarningViewModel warningViewModel = new WarningViewModel();

            //get teacher info
            string tId = User.Identity.GetUserId().ToString();
            var teacher = context.Teacher.Where(t => t.teacherId == tId).FirstOrDefault();

            //get all academic years
            var academicYears = context.AcademicYear.ToList();
            foreach (var getActive in academicYears)
            {
                //get start and end dates to check if today is in the middle
                string[] duration = getActive.duration.Split('-');
                if (!(DateTime.Compare(DateTime.Now, DateTime.Parse(duration[0])) < 0 || DateTime.Compare(DateTime.Now, DateTime.Parse(duration[1])) > 0))
                {
                    //get students with three or more absence records in active academic years and under the unitleader management 
                    var students = context.AbsenceRecord.Where(a => a.academicPeriod.StartsWith(getActive.academicYearName) && a.count>=3 && a.count<5 && a.student.sectionName.StartsWith(teacher.grade.ToString())).ToList();

                    if (students.Count != 0)
                    {
                        foreach(var getStudents in students)
                        {
                            string previousDate = DateTime.Now.Subtract(TimeSpan.FromDays(1)).ToShortDateString();
                            var previousDay = context.AbsenceRecord.Where(a=>a.recordDate==previousDate && a.studentId==getStudents.studentId);
                            warningViewModel.eligible = context.Student.Where(s => s.studentId == getStudents.studentId).ToList();
                        }   
                    }
                }
            }

         return View();
        }
        }
}
using LCCS_School_Parent_Communication_System.Additional_Class;
using LCCS_School_Parent_Communication_System.Identity;
using LCCS_School_Parent_Communication_System.Models;
using LCCS_School_Parent_Communication_System.viewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
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

        public ActionResult AddLateComer(string id)
        {

            LateComerViewModel lateComerViewModel = new LateComerViewModel();

            //check if current day is not weekend
            if (!(DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday))
            {
                //show conformation message
                lateComerViewModel.ID = Int32.Parse(id);
                ViewBag.message = "Are You Sure Do You Want To Declare the student As a Late Comer?";
            }
            else
            {
                //error message weekend
                ViewBag.error = "You are Not Allowed To Access on the Weekend";
            }

            return PartialView("AddLateComer",lateComerViewModel);
        }

        [HttpPost]
        public ActionResult AddLateComer(LateComerViewModel lateComerViewModel)
        {

            //basic objects
            ApplicationDbContext context = new ApplicationDbContext();
            Suspension suspension = new Suspension();
            Collection collection = new Collection();
            LateComer lateComer = new LateComer();


            int count = 0;
            //get unitleader info
            string tId = User.Identity.GetUserId().ToString();
            var teacher = context.Teacher.Where(t => t.teacherId == tId).FirstOrDefault();

            //get how mach time the student is late and increment it by 1
            var lateRecord = context.LateComer.Where(l => l.studentId == lateComerViewModel.ID).ToList();
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

            //check today is bounded in a quarter
            if (collection.currentQuarter(lateComerViewModel.ID) == " ")
            {
                //error message
                ViewBag.posterror = "Time is not Bounded in a Quarter";
            }
            else
            {
                //check if latecomer record exist for today
                DateTime currentDate = DateTime.Now.Date;
                var duplicate = context.LateComer.Where(l => l.lateDate ==currentDate && l.studentId==lateComerViewModel.ID).FirstOrDefault();
                if (duplicate==null)
                {
                    //populate latecomer object
                    lateComer.academicPeriod = collection.currentQuarter(lateComerViewModel.ID);
                    lateComer.dayCount = count + 1;
                    lateComer.lateDate = DateTime.Now.Date;
                    lateComer.studentId = lateComerViewModel.ID;

                    //save latecomer record
                    context.LateComer.Add(lateComer);
                    int result = context.SaveChanges();

                    if (result > 0)
                    {
                        //send warning message if the counter reaches 3
                        if (lateComer.dayCount == 3 || lateComer.dayCount == 7 || lateComer.dayCount == 11)
                        {
                            //warning object
                            Warning warning = new Warning();

                            //populate warning object
                            warning.studentId = lateComerViewModel.ID;
                            warning.WarningReadStatus = "No";
                            warning.grade = teacher.grade;
                            warning.warningType = "LateComer";
                            warning.academicYear = collection.currentQuarter(lateComerViewModel.ID);
                            warning.warningDate = DateTime.Now.Date;

                            //save warning
                            context.Warning.Add(warning);
                            context.SaveChanges();
                        }
                        else if (lateComer.dayCount == 4 || lateComer.dayCount == 8 || lateComer.dayCount == 12)
                        {
                            //suspend student for two days
                            suspension.studentId = lateComerViewModel.ID;
                            suspension.startDate = DateTime.Now;
                            do
                            {
                                suspension.startDate = suspension.startDate.AddDays(1);
                            }
                            while (suspension.startDate.DayOfWeek == DayOfWeek.Saturday || suspension.startDate.DayOfWeek == DayOfWeek.Sunday);

                            suspension.endDate = suspension.startDate;
                            do
                            {
                                suspension.endDate = suspension.endDate.AddDays(1);
                            }
                            while (suspension.endDate.DayOfWeek == DayOfWeek.Saturday || suspension.endDate.DayOfWeek == DayOfWeek.Sunday);

                            //save suspension record
                            context.Suspension.Add(suspension);
                            context.SaveChanges();
                        }

                        //success message
                        ViewBag.complete = "Latecomer Recorded Successfully";
                    }
                    else
                    {
                        //error message
                        ViewBag.error = "Failed to Record Latecomer";
                    }
                }
                else
                {
                    //error message duplicate
                    ViewBag.error = "Student has Already been Labeled as a Latecomer.";
                }
            }


            return PartialView("AddLateComer");
        }

        public ActionResult LateComerManagement()
        {

            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            Suspension suspension = new Suspension();
            LateComerViewModel lateComerViewModel = new LateComerViewModel();
            lateComerViewModel.students = new List<Student>();
            LateComer lateComer = new LateComer();
            Collection collection = new Collection();

            if (!(DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday))
            {

                //get unitleader info
                string tId = User.Identity.GetUserId().ToString();
                var teacher = context.Teacher.Where(t => t.teacherId == tId).FirstOrDefault();

                //get all academic years
                var academicYears = context.AcademicYear.ToList();
                foreach (var getActive in academicYears)
                {
                    //get start and end dates to check if today is in the middle
                    if (!(DateTime.Compare(DateTime.Now, getActive.durationStart) < 0 || DateTime.Compare(DateTime.Now, getActive.durationEnd) > 0))
                    {
                        //search student by grade in active academic years
                        lateComerViewModel.students = context.Student.Where(s => s.academicYearId == getActive.academicYearName && s.sectionName.StartsWith(teacher.grade.ToString())).ToList();
                    }
                }

                if (lateComerViewModel.students.Count != 0)
                {
                    //check if student is suspended or not
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
                                context.SaveChanges();
                            }
                            else
                            {
                                //remove student from search list
                                lateComerViewModel.students.Remove(checkStudent);
                            }
                        }
                    }

                }
            }
            else
            {
                //error message weekend
                ViewBag.message = "You are Not Allowed To Access on the Weekend";
            }

            return View(lateComerViewModel);
        }
        

        public ActionResult SendWarning(string id)
        {
            WarningViewModel warningViewModel = new WarningViewModel();

            //check current day is not weekend
            if (!(DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday))
            {
                //conformation message
                warningViewModel.ID = Int32.Parse(id);
                ViewBag.message = "Are You Sure Do You Want To Send Warning Message For Selected Student?";
            }
            else
            {
                //error message weekend
                ViewBag.error = "You are Not Allowed To Access on the Weekend";
            }

            return PartialView("SendWarning",warningViewModel);
        }

        [HttpPost]
        public ActionResult SendWarning(WarningViewModel warningViewModel)
        {
            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            Warning warningobj = new Warning();
            Collection collection = new Collection();

            string quarter = collection.currentQuarter(warningViewModel.ID);

            //get teacher grade from login info
            string tId = User.Identity.GetUserId().ToString();
            var teacher = context.Teacher.Where(t => t.teacherId == tId).FirstOrDefault();

            //populate warning object
            warningobj.academicYear = quarter;
            warningobj.warningDate = DateTime.Now.Date;
            warningobj.WarningReadStatus = "No";
            warningobj.warningType = "Atendance";
            warningobj.studentId = warningViewModel.ID;
            warningobj.grade = teacher.grade;

            context.Warning.Add(warningobj);
            int result=context.SaveChanges();
            if (result > 0)
            {
                //warning send message
                ViewBag.complete = "Warning Send Successfully";
            }
            else
            {
                //fail message
                ViewBag.posterror = "Failed to Send Warning!!";
            }
            
            return PartialView("SendWarning",warningViewModel);
        }

        public ActionResult WarningManagement()
        {
            //object declaration
            WarningViewModel warningViewModel = new WarningViewModel();
            warningViewModel.eligible = new List<Student>();
            warningViewModel.nonViewed = new List<Warning>();

            //check current day is not weekend
            if (!(DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday))
            {
                //populate eligible and nonViewed objects
                warningViewModel = nonViewedWarnings();
                warningViewModel.eligible = eligibleStudents();
                
            }
            else
            {
                //error message weekend
                ViewBag.message = "You are Not Allowed To Access on the Weekend";
            }
            return View(warningViewModel);
        }

        public ActionResult EvidenceApproval(int id)
        {
            //object declartion
            ApplicationDbContext context = new ApplicationDbContext();
            var evidenceApprovalViewModel = new EvidenceApprovalViewModel();
            evidenceApprovalViewModel.days = new List<string>();

            ViewBag.view = true;

            //search evidence record
            var evidence = context.Evidence.Find(id);

            var yesterday = DateTime.Now.Subtract(TimeSpan.FromDays(1));

            //get all absence days for the specified evidence
            do
            {
                
                var daysRecord = context.AbsenceRecord.Where(a => a.studentId == evidence.parent.student.studentId && a.recordDate == yesterday.Date).FirstOrDefault();
                if (daysRecord != null)
                {
                    evidenceApprovalViewModel.days.Add(daysRecord.recordDate.Date.ToShortDateString());

                    do
                    {
                        yesterday = yesterday.Subtract(TimeSpan.FromDays(1));
                    }
                    while (yesterday.DayOfWeek == DayOfWeek.Saturday || yesterday.DayOfWeek == DayOfWeek.Sunday);
                }
                else
                {
                    break;
                }

            }
            while (true);

            evidenceApprovalViewModel.Id = id;
            evidenceApprovalViewModel.document = evidence.evidenceDocument;

            return View(evidenceApprovalViewModel);
        }

        [HttpPost]
        public ActionResult EvidenceApproval(EvidenceApprovalViewModel evidenceApprovalViewModel,string[] days)
        {
            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            HomeroomTeacherMethod homeroomTeacherMethod = new HomeroomTeacherMethod();

            ViewBag.view = false;

            var evidence = context.Evidence.Find(evidenceApprovalViewModel.Id);
            evidence.approvalStatus = "Seen";
            
            context.SaveChanges();

            if (days != null)
            {
                for (int i = 0; i < days.Length; i++)
                {
                    DateTime date = DateTime.Parse(days[i]);
                    var absenceRecord = context.AbsenceRecord.Where(a => a.recordDate == date.Date && a.studentId == evidence.parent.student.studentId).FirstOrDefault();
                    absenceRecord.evidenceFlag = "AcceptableReason";
                    context.SaveChanges();
                }

                string quarter = homeroomTeacherMethod.whichQuarter(evidence.parent.student.academicYearId);
                var record = context.AbsenceRecord.Where(a => a.academicPeriod == evidence.parent.student.academicYearId + "-" + quarter && a.studentId == evidence.parent.studentId).ToList();

                record[record.Count - 1].count = record[record.Count - 1].count - days.Length;
                context.SaveChanges();
            }

            ViewBag.complete = "Task Completed Successfully";

            return View(evidenceApprovalViewModel);
        }

        public ActionResult StudentEvidence()
        {
            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            List<Student> students = new List<Student>();
            List<Evidence> evidence = new List<Evidence>();

            var currentDate = DateTime.Now.Date;

            //get teacher grade from login info
            string tId = User.Identity.GetUserId().ToString();
            var teacher = context.Teacher.Where(t => t.teacherId == tId).FirstOrDefault();

            //check current date is not weekend
            if (!(DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday))
            {
                //get evidence that parents send today
                evidence = context.Evidence.Where(e => e.parent.student.sectionName.StartsWith(teacher.grade.ToString()) && e.approvalStatus == "Provided" && e.dateUpload == currentDate).ToList();
            }
            else
            {
                //error message weekend
                ViewBag.message = "You are Not Allowed To Access on the Weekend";
            }


            return View(evidence);
        }


        public ActionResult success()
        {

            return PartialView("success");
        }

        public List<Student> eligibleStudents()
        {
            //basic objects
            ApplicationDbContext context = new ApplicationDbContext();
            Warning warning = new Warning();
            WarningViewModel warningViewModel = new WarningViewModel();
            HomeroomTeacherMethod homeroomTeacherMethod = new HomeroomTeacherMethod();
            warningViewModel.eligible = new List<Student>();

            //get teacher info
            string tId = User.Identity.GetUserId().ToString();
            var teacher = context.Teacher.Where(t => t.teacherId == tId).FirstOrDefault();

            //get all academic years
            var academicYears = context.AcademicYear.ToList();
            if (academicYears.Count > 0)
            {
                foreach (var getActive in academicYears)
                {
                    //get start and end dates to check if today is in the middle
                    if (!(DateTime.Compare(DateTime.Now, getActive.durationStart) < 0 || DateTime.Compare(DateTime.Now, getActive.durationEnd) > 0))
                    {
                        string quarter = homeroomTeacherMethod.whichQuarter(getActive.academicYearName);

                        //get students with three or more but less than 5 absence records in active academic years and under the unitleader management 
                        var students = context.AbsenceRecord.Where(a => a.academicPeriod == getActive.academicYearName + "-" + quarter && a.count >= 3 && a.count < 5 && a.student.sectionName.StartsWith(teacher.grade.ToString())).ToList();

                        if (students.Count != 0)
                        {
                            foreach (var getStudents in students)
                            {
                                //check prevoius day attendance to be sure about no evidence is provided
                                DateTime yesterday = DateTime.Now;
                                do
                                {
                                    yesterday = yesterday.Subtract(TimeSpan.FromDays(1));
                                }
                                while (yesterday.DayOfWeek == DayOfWeek.Sunday || yesterday.DayOfWeek == DayOfWeek.Saturday);
                                //string previousDate = yesterday.ToShortDateString();
                                var previousDay = context.AbsenceRecord.Where(a => a.recordDate == yesterday.Date && a.studentId == getStudents.studentId).FirstOrDefault();

                                if (previousDay == null)
                                {
                                    //check warning message is already their
                                    var inWarning = context.Warning.Where(w => w.academicYear == getActive.academicYearName + "-" + quarter && w.studentId == getStudents.studentId).FirstOrDefault();
                                    if (inWarning == null)
                                    {
                                        //populate the student as eligible for warning
                                        var studentEligible = context.Student.Where(s => s.studentId == getStudents.studentId).FirstOrDefault();
                                        warningViewModel.eligible.Add(studentEligible);
                                    }
                                }

                            }
                        }
                    }
                }
            }
            return warningViewModel.eligible;

        }

        public WarningViewModel nonViewedWarnings()
        {
            //basic objects
            ApplicationDbContext context = new ApplicationDbContext();
            Warning warning = new Warning();
            WarningViewModel warningViewModel = new WarningViewModel();
            HomeroomTeacherMethod homeroomTeacherMethod = new HomeroomTeacherMethod();
            warningViewModel.nonViewed = new List<Warning>();
            warningViewModel.parentName = new List<string>();
            warningViewModel.parentPhone = new List<string>();

            //get teacher info
            string tId = User.Identity.GetUserId().ToString();
            var teacher = context.Teacher.Where(t => t.teacherId == tId).FirstOrDefault();

            //get all academic years
            var academicYears = context.AcademicYear.ToList();

            if (academicYears.Count > 0)
            {
                foreach (var getActive in academicYears)
                {
                    //get start and end dates to check if today is in the middle
                    if (!(DateTime.Compare(DateTime.Now, getActive.durationStart) < 0 || DateTime.Compare(DateTime.Now, getActive.durationEnd) > 0))
                    {
                        //get current quarter and students who register for that academic year who are managed by this unitleader
                        string quarter = homeroomTeacherMethod.whichQuarter(getActive.academicYearName);
                        var studentInGrade = context.Student.Where(s => s.sectionName.StartsWith(teacher.grade.ToString()) && s.academicYearId == getActive.academicYearName).ToList();

                        if (studentInGrade.Count != 0)
                        {
                            foreach (var getStudent in studentInGrade)
                            {
                                //get warnings that are not viewed by parents
                                var warnings = context.Warning.Where(w => w.WarningReadStatus == "No" && w.studentId == getStudent.studentId && w.academicYear == getActive.academicYearName + "-" + quarter).ToList();

                                if (warnings.Count != 0)
                                {
                                    //populate the nonviewed warnings to the viewmodel object

                                    foreach(var getWarnings in warnings)
                                    {
                                        warningViewModel.nonViewed.Add(getWarnings);

                                        //parent information
                                        var parent = context.Parent.Where(p => p.studentId == getStudent.studentId).ToList();

                                        if (parent.Count != 0)
                                        {
                                            if (parent.Count == 1)
                                            {
                                                warningViewModel.parentName.Add(parent[0].user.fullName);
                                                warningViewModel.parentPhone.Add(parent[0].user.PhoneNumber);
                                            }
                                            else if (parent.Count == 2)
                                            {
                                                warningViewModel.parentName.Add(parent[0].user.fullName + "/" + parent[1].user.fullName);
                                                warningViewModel.parentPhone.Add(parent[0].user.PhoneNumber + "/" + parent[1].user.PhoneNumber);
                                            }
                                        }
                                    }
                                    
                                }
                            }
                        }
                    }
                }
            }
            return warningViewModel;
        }
    }
}
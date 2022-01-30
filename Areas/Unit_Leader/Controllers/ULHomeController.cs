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

        public ActionResult LateComerManagement()
        {
            //viewbag element used for UI
            ViewBag.search = false;
            return View();
        }
        [HttpPost]
        public ActionResult LateComerManagement(LateComerViewModel lateComerViewModel, string id, string late, string search)
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


            if(!(DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday))
            {
                if (ModelState.IsValid || late != null)
                {
                    //get unitleader info
                    string tId = User.Identity.GetUserId().ToString();
                    var teacher = context.Teacher.Where(t => t.teacherId == tId).FirstOrDefault();

                    //check search button is clicked
                    if (search != null)
                    {

                        //get all academic years
                        var academicYears = context.AcademicYear.ToList();
                        int x = 0;
                        foreach (var getActive in academicYears)
                        {
                            //get start and end dates to check if today is in the middle
                            if (!(DateTime.Compare(DateTime.Now, getActive.durationStart) < 0 || DateTime.Compare(DateTime.Now, getActive.durationEnd) > 0))
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
                                warning.academicYear = collection.currentQuarter(ID);
                                warning.warningDate = DateTime.Now.Date;

                                //save warning
                                context.Warning.Add(warning);
                                context.SaveChanges();
                            }
                            else if (lateComer.dayCount == 4 || lateComer.dayCount == 8 || lateComer.dayCount == 12)
                            {
                                //suspend student for two days
                                suspension.studentId = ID;
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
                            ViewBag.message = "LateComer Recorded Successfully";
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

        public ActionResult WarningManagement()
        {

            ////basic objects
            //ApplicationDbContext context = new ApplicationDbContext();
            //Warning warning = new Warning();
            //WarningViewModel warningViewModel = new WarningViewModel();
            //HomeroomTeacherMethod homeroomTeacherMethod = new HomeroomTeacherMethod();

            ////get teacher info
            //string tId = User.Identity.GetUserId().ToString();
            //var teacher = context.Teacher.Where(t => t.teacherId == tId).FirstOrDefault();

            ////get all academic years
            //var academicYears = context.AcademicYear.ToList();
            //foreach (var getActive in academicYears)
            //{
            //    //get start and end dates to check if today is in the middle
            //    string[] duration = getActive.duration.Split('-');
            //    if (!(DateTime.Compare(DateTime.Now, DateTime.Parse(duration[0])) < 0 || DateTime.Compare(DateTime.Now, DateTime.Parse(duration[1])) > 0))
            //    {
            //        string quarter = homeroomTeacherMethod.whichQuarter(getActive.academicYearName);

            //        //get students with three or more but less than 5 absence records in active academic years and under the unitleader management 
            //        var students = context.AbsenceRecord.Where(a => a.academicPeriod == getActive.academicYearName + "-" + quarter && a.count >= 3 && a.count < 5 && a.student.sectionName.StartsWith(teacher.grade.ToString())).ToList();

            //        if (students.Count != 0)
            //        {
            //            foreach (var getStudents in students)
            //            {
            //                //check prevoius day attendance to be sure about no evidence is provided
            //                string previousDate = DateTime.Now.Subtract(TimeSpan.FromDays(1)).ToShortDateString();
            //                var previousDay = context.AbsenceRecord.Where(a => a.recordDate == previousDate && a.studentId == getStudents.studentId);

            //                if (previousDay == null)
            //                {
            //                    //check warning message is already their
            //                    var inWarning = context.Warning.Where(w => w.academicYear == getActive.academicYearName + "-" + quarter && w.studentId == getStudents.studentId).FirstOrDefault();
            //                    if (inWarning == null)
            //                    {
            //                        //populate the student as eligible for warning
            //                        warningViewModel.eligible = context.Student.Where(s => s.studentId == getStudents.studentId).ToList();
            //                    }
            //                }

            //            }
            //        }
            //    }
            //}

            ////----------------------=----------------------------------


            ////get teacher info
            //string tId = User.Identity.GetUserId().ToString();
            //var teacher = context.Teacher.Where(t => t.teacherId == tId).FirstOrDefault();



            ////get all academic years
            //var academicYears = context.AcademicYear.ToList();
            //foreach (var getActive in academicYears)
            //{
            //    //get start and end dates to check if today is in the middle
            //    string[] duration = getActive.duration.Split('-');
            //    if (!(DateTime.Compare(DateTime.Now, DateTime.Parse(duration[0])) < 0 || DateTime.Compare(DateTime.Now, DateTime.Parse(duration[1])) > 0))
            //    {
            //        //get current quarter and students who register for that academic year who are managed by this unitleader
            //        string quarter = homeroomTeacherMethod.whichQuarter(getActive.academicYearName);
            //        var studentInGrade = context.Student.Where(s => s.sectionName.StartsWith(teacher.grade.ToString()) && s.academicYearId == getActive.academicYearName).ToList();

            //        if (studentInGrade.Count != 0)
            //        {
            //            foreach (var getStudent in studentInGrade)
            //            {
            //                //get warnings that are not viewed by parents
            //                var warnings = context.Warning.Where(w => w.WarningReadStatus == "No" && w.studentId == getStudent.studentId && w.academicYear == getActive.academicYearName + "-" + quarter).ToList();

            //                if (warnings.Count != 0)
            //                {
            //                    foreach (var getNonViewed in warnings)
            //                    {
            //                        //populate the nonviewed warnings to the viewmodel object
            //                        warningViewModel.nonViewed = context.Student.Where(s => s.studentId == getNonViewed.studentId).ToList();
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            //basic objects
            WarningViewModel warningViewModel = new WarningViewModel();
            warningViewModel.eligible = new List<Student>();
            warningViewModel.nonViewed = new List<Warning>();
            if (!(DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday))
            {
                //populate eligible and nonViewed objects
                warningViewModel.eligible = eligibleStudents();
                warningViewModel.nonViewed = nonViewedWarnings();
            }
            else
            {
                //error message weekend
                ViewBag.message = "You are Not Allowed To Access on the Weekend";
            }
            return View(warningViewModel);
        }

        [HttpPost]
        public ActionResult WarningManagement(WarningViewModel warningViewModel,string id,string warning)
        {
            //basic objects
            ApplicationDbContext context = new ApplicationDbContext();
            Warning warningobj = new Warning();
            Collection collection = new Collection();
            warningViewModel.eligible = new List<Student>();
            warningViewModel.nonViewed = new List<Warning>();

            if(!(DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday))
            {
                //check if warning button is clicked
                if (warning != null)
                {
                    //parse the passed id to int
                    int ID = Int32.Parse(id);
                    string quarter = collection.currentQuarter(ID);

                    //get teacher grade from login info
                    string tId = User.Identity.GetUserId().ToString();
                    var teacher = context.Teacher.Where(t => t.teacherId == tId).FirstOrDefault();

                    //populate warning object
                    warningobj.academicYear = quarter;
                    warningobj.warningDate = DateTime.Now.Date;
                    warningobj.WarningReadStatus = "No";
                    warningobj.warningType = "Atendance";
                    warningobj.studentId = ID;
                    warningobj.grade = teacher.grade;

                    //warning send message
                    ViewBag.message = "Warning Send Successfully";
                }

                //populate eligible and nonViewed objects
                warningViewModel.eligible = eligibleStudents();
                warningViewModel.nonViewed = nonViewedWarnings();
            }
            else
            {
                //error message weekend
                ViewBag.message = "You are Not Allowed To Access on the Weekend";
            }
            return View();
        }

        public ActionResult EvidenceApproval(int id)
        {
            //basic objects
            ApplicationDbContext context = new ApplicationDbContext();
            var evidenceApprovalViewModel = new EvidenceApprovalViewModel();
            evidenceApprovalViewModel.days = new List<string>();

            ViewBag.view = true;

            //search evidence record
            var evidence = context.Evidence.Find(id);

            var yesterday = DateTime.Now.Subtract(TimeSpan.FromDays(1));

            do
            {
                //string yesterdayDate = yesterday.ToShortDateString();
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
            //list student as a link to open evidence page

            ApplicationDbContext context = new ApplicationDbContext();

            //ViewBag.view = true;
            ////change the uploaded file to byte array
            //int length = file.ContentLength;
            //byte[] upload = new byte[length];
            //file.InputStream.Read(upload, 0, length);

            ////populate the evidence object
            //evidence.evidenceDocument = upload;
            //evidence.parentId = "sdf";
            //evidence.dateUpload = DateTime.Now.ToShortDateString();
            //evidence.approvalStatus = "Provided";

            ////save the record
            //context.Evidence.Add(evidence);
            //context.SaveChanges();

            //ViewBag.er = test[0];

            var evidence = context.Evidence.Find(evidenceApprovalViewModel.Id);
            evidence.approvalStatus = "Seen";

            context.SaveChanges();

            for(int i = 0; i < days.Length; i++)
            {
                DateTime date = DateTime.Parse(days[i]);
                var absenceRecord = context.AbsenceRecord.Where(a => a.recordDate == date.Date && a.studentId == evidence.parent.student.studentId).FirstOrDefault();
                absenceRecord.evidenceFlag = "AcceptableReason";
                context.SaveChanges();
            }

            return View("StudentEvidence");
        }

        public ActionResult StudentEvidence()
        {
            //basic objects
            ApplicationDbContext context = new ApplicationDbContext();
            List<Student> students = new List<Student>();
            List<Evidence> evidence = new List<Evidence>();

            var currentDate = DateTime.Now.Date;

            //get teacher grade from login info
            string tId = User.Identity.GetUserId().ToString();
            //string tId = "03abb6d5-db37-4e92-bf04-69ef0a4447c0";
            var teacher = context.Teacher.Where(t => t.teacherId == tId).FirstOrDefault();


            if (!(DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday))
            {
                //get evidence that parents send today
                evidence = context.Evidence.Where(e => e.parent.student.sectionName.StartsWith(teacher.grade.ToString()) && e.approvalStatus == "Provided" && e.dateUpload == currentDate).ToList();
                //if (evidence.Count != 0)
                //{
                //    foreach(var getStudent in evidence)
                //    {
                //        var student= context.Student.Where(s=> s.studentId==getStudent.parent.studentId).FirstOrDefault();
                //        students.Add(student);
                //    } 
                //}
            }
            else
            {
                //error message weekend
                ViewBag.message = "You are Not Allowed To Access on the Weekend";
            }


            return View(evidence);
        }

        //public ActionResult EvidenceCheck()
        //{
        //    //ApplicationDbContext context = new ApplicationDbContext();
        //    //var evidence = context.Evidence.Find(9);
        //    //byte[] byteArray = evidence.evidenceDocument;
        //    /////new FileContentResult(byteArray, "application/pdf");
        //    //return File(byteArray, "application/pdf");
        //    return View();



        //}
        //public FileResult EvidenceCheck1()
        //{
        //    ApplicationDbContext context = new ApplicationDbContext();
        //    var evidence = context.Evidence.Find(9);
        //    byte[] byteArray = evidence.evidenceDocument;
        //    ///new FileContentResult(byteArray, "application/pdf");
        //    return File(byteArray, "application/pdf");



        //}


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
                            while (yesterday.DayOfWeek==DayOfWeek.Sunday || yesterday.DayOfWeek==DayOfWeek.Saturday);
                            //string previousDate = yesterday.ToShortDateString();
                            var previousDay = context.AbsenceRecord.Where(a => a.recordDate == yesterday.Date && a.studentId == getStudents.studentId);

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

            return warningViewModel.eligible;

        }

        public List<Warning> nonViewedWarnings()
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
                                warningViewModel.nonViewed.Add(warning);

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

            return warningViewModel.nonViewed;
        }
    }
}
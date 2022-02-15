using LCCS_School_Parent_Communication_System.Identity;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LCCS_School_Parent_Communication_System.viewModels;
using LCCS_School_Parent_Communication_System.Models;
using LCCS_School_Parent_Communication_System.Additional_Class;
namespace LCCS_School_Parent_Communication_System.Areas.Parent.Controllers
{
    [Authorize(Roles = "Parent")]

    public class PHomeController : Controller
    {
        // GET: Parent/PHome
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult viewAttendance()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string currentuser =User.Identity.GetUserId();
            AbsenceRecord ab = new AbsenceRecord();

            return View(new calanderEvents());
        }
        public JsonResult GetEvents()
        {
            calanderEvents cv = new calanderEvents();
            ApplicationDbContext db = new ApplicationDbContext();
            List<calanderEvents> events = new List<calanderEvents>();
            List<Schedule> ls = new List<Schedule>();
            string currentUserId = User.Identity.GetUserId();
            Models.Parent p = new Models.Parent();
            p = db.Parent.Where(r => r.parentId == currentUserId).FirstOrDefault();
            Student s = new Student();
            s = db.Student.Where(ax => ax.studentId == p.studentId).FirstOrDefault();
           List<AbsenceRecord>  ab = new List<AbsenceRecord>();
            List<LateComer> lc = new List<LateComer>();
            
            if (s != null) {
                string k = s.sectionName;
                string temp = k.Substring(0, k.Length-1);
                int gr = int.Parse(temp);
                ls = db.Schedule.Where(ax => ax.grade == gr).ToList();
                if (ls != null)
                {
                    foreach(var ks in ls)
                    {
                        events.Add(new calanderEvents()
                        {
                            id = ks.scheduleId,
                            title = ks.scheduleFor+"-"+ks.subject,
                            start = ks.scheduleDate.ToShortDateString(),
                            end = ks.scheduleDate.ToShortDateString(),
                            allDay = true,
                            backgroundColor="blue",
                        }

                      );
                    }
                }
            }
            ab = db.AbsenceRecord.Where(a=>a.studentId==p.studentId).ToList();
            lc = db.LateComer.Where(a => a.studentId == p.studentId).ToList();
            if (ab.Count() != 0) 
            {
                foreach(var g in ab)
                {
                    if (g.evidenceFlag == "Absent" || g.evidenceFlag==null) { 
                    events.Add(new calanderEvents()
                    {
                        id =g.recordId,
                        title = "Absent",
                        start = g.recordDate.ToShortDateString(),
                        end =g.recordDate.ToShortDateString(),
                        allDay = true,
                        backgroundColor="red",
                    }

                        );
                    }
                    else if (g.evidenceFlag == "AcceptableReason")
                    {
                        events.Add(new calanderEvents()
                        {
                            id = g.recordId,
                            title = "Evident",
                            start = g.recordDate.ToShortDateString(),
                            end = g.recordDate.ToShortDateString(),
                            allDay = true,
                            backgroundColor="green"
                        }

                                                );
                    }
                }
            }
            if (lc.Count != 0)
            {
                foreach (var m in lc)
                {
                    events.Add(new calanderEvents()
                    {
                        id = m.lateId,
                        title = "Late",
                        start = m.lateDate.ToShortDateString(),
                        end = m.lateDate.ToShortDateString(),
                        allDay = true,
                        backgroundColor="black",
                    }
                        );
                }
            }
           

            int x = 20;
            return Json(events, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EvidenceManagement()
        {
            return View();
        }
        [HttpPost]
        public ActionResult EvidenceManagement(HttpPostedFileBase file)
        {
            //object declaration
            Evidence evidence = new Evidence();
            ApplicationDbContext context = new ApplicationDbContext();

            //reference time
            string reference = DateTime.Now.ToShortDateString() + " 10:00 AM";

            //get parent info
            string pId = User.Identity.GetUserId().ToString();
            
            var parent = context.Parent.Where(p => p.parentId == pId).FirstOrDefault();
            var secondParent = context.Parent.Where(p=> p.studentId==parent.studentId && p.parentId!=pId).FirstOrDefault();

            if(!(DateTime.Now.DayOfWeek==DayOfWeek.Sunday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday))
            {
                if (DateTime.Compare(DateTime.Now, DateTime.Parse(reference)) > 0)
                {
                    //get the previous date
                    DateTime yesterday = DateTime.Now;
                    do
                    {
                        yesterday = yesterday.Subtract(TimeSpan.FromDays(1));
                    }
                    while (yesterday.DayOfWeek == DayOfWeek.Sunday || yesterday.DayOfWeek == DayOfWeek.Saturday);
                    //string previousDate = yesterday.ToShortDateString();
                    var absenceRecord = context.AbsenceRecord.Where(a => a.studentId == parent.studentId && a.recordDate == yesterday.Date).FirstOrDefault();

                    //check if the student absence record
                    if (absenceRecord != null)
                    {
                        var currentDay = DateTime.Now.Date;
                        var presenceRecord = context.AbsenceRecord.Where(a => a.studentId == parent.studentId && a.recordDate == currentDay.Date).FirstOrDefault();

                        //check today attendance record
                        if (presenceRecord == null)
                        {
                            Evidence duplicateEvidence;
                            if (secondParent == null)
                            {
                                duplicateEvidence = context.Evidence.Where(e => e.parentId == pId && e.dateUpload == currentDay.Date).FirstOrDefault();
                            }
                            else
                            {
                                duplicateEvidence = context.Evidence.Where(e => (e.parentId == pId && e.dateUpload == currentDay.Date) || (e.parentId == secondParent.parentId && e.dateUpload == yesterday.Date)).FirstOrDefault();
                            }

                            //check if the eviedence is provided once
                            if (duplicateEvidence == null)
                            {
                                if (file.ContentType == "application/pdf")
                                {
                                    //change the uploaded file to byte array
                                    int length = file.ContentLength;
                                    byte[] upload = new byte[length];
                                    file.InputStream.Read(upload, 0, length);

                                    //populate the evidence object
                                    evidence.evidenceDocument = upload;
                                    evidence.parentId = pId;
                                    evidence.dateUpload = DateTime.Now.Date;
                                    evidence.approvalStatus = "Provided";

                                    //save the record
                                    context.Evidence.Add(evidence);
                                    int result=context.SaveChanges();
                                    if (result > 0)
                                    {
                                        //sucess message
                                        ViewBag.complete = "Evidence Uploaded Successfully";
                                    }
                                    else
                                    {
                                        //error message
                                        ViewBag.error = "Failed to Upload Evidence!!";
                                    }
                                    
                                }
                                else
                                {
                                    //error pdf document
                                    ViewBag.error = "Only Pdf Documents are Allowed";
                                }
                                
                            }
                            else
                            {
                                //error duplicate
                                ViewBag.error = "Evidence Already Provided";
                            }
                        }
                        else
                        {
                            //error message send the evidence on student return day
                            ViewBag.error = "Please Send the Evidence on Student Return Date";

                        }
                    }
                    else
                    {
                        //error no absence record found
                        ViewBag.error = "No Valid Absence Record to Send Evidence";
                    }
                }
                else
                {
                    //error message time should pass 10:00
                    ViewBag.error = "Please Send the Evidence After 10:00 AM";
                }
            }
            else
            {
                //error message weekend
                ViewBag.error = "You are Not Allowed To send Evidence on A Weekend";
            }
            return View();
        }

        public ActionResult WarningList()
        {
            //context object
            ApplicationDbContext context = new ApplicationDbContext();

            //get parent info
            string pId = User.Identity.GetUserId().ToString();
          
            var parent = context.Parent.Where(p => p.parentId == pId).FirstOrDefault();

            var warnings = context.Warning.Where(w => w.studentId == parent.studentId && w.WarningReadStatus == "No").ToList();

            return View(warnings);
        }
        
        public ActionResult ViewWarning(int id)
        {
            //context object
            ApplicationDbContext context = new ApplicationDbContext();

            //get the specified warning
            var warning = context.Warning.Find(id);
            warning.WarningReadStatus = "Yes";

            context.SaveChanges();

            return View(warning);
        }
        public ActionResult viewAssignment()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            HomeroomTeacherMethod ht = new HomeroomTeacherMethod();
            List<Assignment> temp = new List<Assignment>();
            Section sec = new Section();
            string currentUserID = User.Identity.GetUserId().ToString();
            Models.Parent p = new Models.Parent();
            List<Assignment> ass = new List<Assignment>();
            GroupStructureAssignment gsa = new GroupStructureAssignment();
            List<GroupAssignment> gr = new List<GroupAssignment>();
            List<Group> grList = new List<Group>();
            DateTime d = DateTime.Now.Date;
            Student s = new Student();
            p = db.Parent.Where(x => x.parentId == currentUserID).FirstOrDefault();
            s = db.Student.Where(x => x.studentId == p.studentId).FirstOrDefault();
            sec = db.Section.Where(x => x.sectionName == s.sectionName && x.academicYearId == s.academicYearId).FirstOrDefault();
            if (s!= null)
            {
               
               string status= ht.whichQuarter(s.academicYearId);
                if(status== "Q1" || status== "Q2" || status== "Q3" || status== "Q4")
                {
                    string currentVal = s.academicYearId + "-" + status;
                    ass = db.Assignment.Where(x => x.sectionID == sec.sectionId && x.yearlyQuarter==currentVal).ToList();
                    foreach(var x in ass)
                    {
                        if (DateTime.Compare(DateTime.Now.Date, x.submissionDate) <= 0 )
                        {
                            if (x.assignmentType == "Individual") {
                                temp.Add(x);
                            }
                           

                            else if (x.assignmentType == "Group")
                            {
                                gsa = db.GroupStructureAssignment.Where(ax => ax.assignmentId == x.assignmentId).FirstOrDefault();
                                if (gsa != null)
                                {
                                    temp.Add(x);
                                }
                                gr = db.GroupAssignment.Where(ax => ax.assignmentId == x.assignmentId).ToList();
                                if (gr != null)
                                {
                                    foreach(var m in gr)
                                    {
                                        grList = db.Group.Where(ax => ax.groupId == m.grId).ToList();
                                        foreach(var ix in grList)
                                        {
                                            var std = db.StudentGroupList.Where(ax => ax.groupId == ix.groupId && ax.studentId == s.studentId).FirstOrDefault();
                                            if (std != null)
                                            {
                                                temp.Add(x);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        
                    }
                    



                }
                else
                {
                    string viewBAGMEssage = "gap,error";
                }
                
                
            }

            return View(temp);
        }
     
    
        
        public ActionResult viewAssignmentDetails()
        {
            
            Assignment ass = new Assignment();
           

                
            return View(ass);
        }
        [HttpPost]
        public ActionResult viewAssignmentDetails(string idk)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Assignment ass = new Assignment();
            int idx = int.Parse(idk);
            ass = db.Assignment.Where(x => x.assignmentId == idx).FirstOrDefault();


            return View(ass);
        }

        public ActionResult ViewResult()
        {
            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            UpdateResultViewModel updateResultViewModel = new UpdateResultViewModel();
            updateResultViewModel.results = new List<Result>();

            Section identifyYear = new Section();

            //get parent information
            string pId = User.Identity.GetUserId().ToString();
            var parent = context.Parent.Find(pId);

            //get all results
            var results = context.Result.Where(r=>r.studentId==parent.studentId).ToList();

            if (results.Count!=0)
            {
                foreach(var getResults in results)
                {
                    string[] yearQuarter = getResults.academicYear.Split('-');

                    if (yearQuarter[1]=="Q1")
                    {
                        yearQuarter[0] = "Quarter1";
                    }
                    else if (yearQuarter[1] == "Q2")
                    {
                        yearQuarter[0] = "Quarter2";
                    }
                    else if (yearQuarter[1] == "Q3")
                    {
                        yearQuarter[0] = "Quarter3";
                    }
                    else if (yearQuarter[1] == "Q4")
                    {
                        yearQuarter[0] = "Quarter4";
                    }
                    getResults.academicYear = "Grade " + getResults.grade.ToString() + '-' + yearQuarter[0];
                    updateResultViewModel.results.Add(getResults);

                }
            }

            return View(updateResultViewModel);
        }

        public ActionResult GenerateGeneralReport()
        {
            
            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            HomeroomTeacherMethod homeroomTeacherMethod = new HomeroomTeacherMethod();
            Section identifyYear = new Section();
            ReportViewModel reportViewModel = new ReportViewModel();
            reportViewModel.subject = new List<string>();
            reportViewModel.score = new List<int>();
            reportViewModel.outOf = new List<int>();

            //declare counters
            int miss = 0;
            int reason = 0;
            int nonReason = 0;
            int percentSum = 0;
            int resultSum = 0;
            int assesementTotal = 0;
            int completeTotal = 0;
            int incompeleteTotal = 0;
            int reassesementTotal = 0;

            //get parent information
            string pId = User.Identity.GetUserId().ToString();
            var parent = context.Parent.Find(pId);

            //get active acadamic years
            var allAcadamicYears = context.AcademicYear.ToList();

            foreach (var getAcadamicYear in allAcadamicYears)
            {
                //check today is in between start and end date of the specific academic year
                if (!(DateTime.Compare(DateTime.Now, getAcadamicYear.durationStart) < 0 || DateTime.Compare(DateTime.Now, getAcadamicYear.durationEnd) > 0))
                {
                    identifyYear = context.Section.Where(s => s.sectionName==parent.student.sectionName && s.academicYearId == getAcadamicYear.academicYearName).FirstOrDefault();
                    if (identifyYear != null)
                    {
                        break;
                    }
                }
            }

            //current acadamic year and quarter
            var getYear = context.AcademicYear.Find(identifyYear.academicYearId);
            var quarter = homeroomTeacherMethod.whichQuarter(identifyYear.academicYearId);

            //result report
            var results = context.Result.Where(r=>r.studentId==parent.studentId && r.academicYear==getYear.academicYearName+"-"+quarter).ToList();
            List<string> uniqueSubject = new List<string>();

            //identify subjects
            if (results.Count!=0)
            {
                foreach (var getSubject in results)
                {
                    uniqueSubject.Add(getSubject.teacher.subject);
                }

                reportViewModel.subject = uniqueSubject.Distinct().ToList();

                //get results of specific subject
                foreach (var getResults in reportViewModel.subject)
                {
                    var subjectResult = context.Result.Where(r => r.teacher.subject == getResults && r.studentId == parent.studentId && r.academicYear == getYear.academicYearName + "-" + quarter).ToList();
                    

                    foreach (var getDetails in subjectResult)
                    {
                        percentSum += getDetails.percent;
                        resultSum += getDetails.result;

                        
                        if (getDetails.feedback == "Student is Absent On the Assesement Day")
                        {
                            miss++;
                        }
                    }

                    reportViewModel.score.Add(resultSum);
                    reportViewModel.outOf.Add(percentSum);

                }
            }

            //absence report

            //get absent and late records
            var absenceRecord = context.AbsenceRecord.Where(a=>a.academicPeriod == getYear.academicYearName + "-" + quarter).ToList();
            var lateRecord = context.LateComer.Where(l => l.academicPeriod == getYear.academicYearName + "-" + quarter).ToList();

            //count reasonable and non reasonable absences 
            if (absenceRecord.Count != 0)
            {
                foreach (var getAbsent in absenceRecord)
                {
                    if (getAbsent.evidenceFlag == null)
                    {
                        nonReason++;
                    }
                    else
                    {
                        reason++;
                    }
                }
            }

            //populate reportViewModel
            reportViewModel.absentDays = absenceRecord.Count;
            reportViewModel.nonReasonable = nonReason;
            reportViewModel.reasonable = reason;
            reportViewModel.lateDays = lateRecord.Count;

            //assesement report

            //get assesement information
            int grade = int.Parse(parent.student.sectionName.Substring(0, parent.student.sectionName.Length - 1));
            var numberOfSchedule = context.Schedule.Where(s=>s.grade==grade && s.academicYear==getYear.academicYearName+"-"+quarter).ToList();
            var numberOfAssignments = context.Assignment.Where(a => a.teacher.grade == grade && a.yearlyQuarter == getYear.academicYearName + "-" + quarter).ToList();

            //count no of complete,incomplete and reassesement assesements
            foreach(var getScheduleNo in numberOfSchedule)
            {
                if (DateTime.Compare(DateTime.Now.Date, getScheduleNo.scheduleDate.Date) > 0)
                {
                    assesementTotal++;
                    var absentSchedule = context.AbsenceRecord.Where(a=>a.recordDate==getScheduleNo.scheduleDate.Date && a.academicPeriod== getYear.academicYearName + "-" + quarter && a.studentId==parent.studentId).FirstOrDefault();
                    if (absentSchedule==null)
                    {
                        completeTotal++;
                    }
                    var reassesementSchedule = context.AbsenceRecord.Where(a => a.recordDate == getScheduleNo.scheduleDate.Date && a.academicPeriod == getYear.academicYearName + "-" + quarter && a.studentId == parent.studentId && a.evidenceFlag== "AcceptableReason").FirstOrDefault();
                    if (reassesementSchedule != null)
                    {
                        reassesementTotal++;
                    }
                }
            }

            foreach (var getAssignmentNo in numberOfAssignments)
            {
                if (DateTime.Compare(DateTime.Now.Date, getAssignmentNo.submissionDate.Date) > 0)
                {
                    assesementTotal++;

                    var absentSchedule = context.AbsenceRecord.Where(a => a.recordDate == getAssignmentNo.submissionDate.Date && a.academicPeriod == getYear.academicYearName + "-" + quarter && a.studentId == parent.studentId && a.evidenceFlag==null).FirstOrDefault();
                    if (absentSchedule == null)
                    {
                        completeTotal++;
                    }
                }   
            }

            incompeleteTotal = assesementTotal - completeTotal;

            //populate reportViewModel
            reportViewModel.totalAssesment = assesementTotal;
            reportViewModel.completeAssesment = completeTotal;
            reportViewModel.incompleteAssesment = incompeleteTotal;
            reportViewModel.reassesement = reassesementTotal;

            return View(reportViewModel);
        }


        public ActionResult viewAnnouncementDetails()
        {
            Announcement av = new Announcement();

            return View(av);
        }
        [HttpPost]
        public ActionResult viewAnnouncementDetails(string annId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Announcement av = new Announcement();
            int annId1 = int.Parse(annId);
            av = db.Announcement.Where(ax => ax.announcementID == annId1).FirstOrDefault();
            av.viewedStatus = 1;
            db.SaveChanges();
            return View(av);
        }



    }
}
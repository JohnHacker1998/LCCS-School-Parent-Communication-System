using LCCS_School_Parent_Communication_System.Identity;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LCCS_School_Parent_Communication_System.viewModels;
using LCCS_School_Parent_Communication_System.Models;

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
            string currentUserId = User.Identity.GetUserId();
            Models.Parent p = new Models.Parent();
            p = db.Parent.Where(r => r.parentId == currentUserId).FirstOrDefault();
           
           List<AbsenceRecord>  ab = new List<AbsenceRecord>();
            List<LateComer> lc = new List<LateComer>();
            ab = db.AbsenceRecord.Where(a=>a.studentId==p.studentId).ToList();
            lc = db.LateComer.Where(a => a.studentId == p.studentId).ToList();
            if (ab.Count() != 0) 
            {
                foreach(var g in ab)
                {
                    if (g.evidenceFlag == "Absent") { 
                    events.Add(new calanderEvents()
                    {
                        id =g.recordId,
                        title = "Absent",
                        start = g.recordDate,
                        end =g.recordDate,
                        allDay = true,
                    }

                        );
                    }
                    else if (g.evidenceFlag == "AcceptableReason")
                    {
                        events.Add(new calanderEvents()
                        {
                            id = g.recordId,
                            title = "Evident",
                            start = g.recordDate,
                            end = g.recordDate,
                            allDay = true,
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
            //basic objects
            Evidence evidence = new Evidence();
            ApplicationDbContext context = new ApplicationDbContext();

            //reference time
            string reference = DateTime.Now.ToShortDateString() + " 10:00 AM";

            //get parent info
            string pId = User.Identity.GetUserId().ToString();
            //string pId = "sdf";
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
                        var currentDay = DateTime.Now.ToShortDateString();
                        var presenceRecord = context.AbsenceRecord.Where(a => a.studentId == parent.studentId && a.recordDate == yesterday.Date).FirstOrDefault();

                        //check today attendance record
                        if (presenceRecord == null)
                        {
                            Evidence duplicateEvidence;
                            if (secondParent == null)
                            {
                                duplicateEvidence = context.Evidence.Where(e => e.parentId == pId && e.dateUpload == yesterday.Date).FirstOrDefault();
                            }
                            else
                            {
                                duplicateEvidence = context.Evidence.Where(e => (e.parentId == pId && e.dateUpload == yesterday.Date) || (e.parentId == secondParent.parentId && e.dateUpload == yesterday.Date)).FirstOrDefault();
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
                                    context.SaveChanges();

                                    //sucess message
                                    ViewBag.message = "Evidence Uploaded Successfully";
                                }
                                else
                                {
                                    //error pdf document
                                    ViewBag.message = "Only Pdf Documents are Allowed";
                                }
                                
                            }
                            else
                            {
                                //error duplicate
                                ViewBag.message = "Their is an Evidence Already Provided";
                            }
                        }
                        else
                        {
                            //error message send the evidence on student return day
                            ViewBag.message = "Please Send the Evidence on Student Return Date";

                        }
                    }
                    else
                    {
                        //error no absence record found
                        ViewBag.message = "No Valid Absence Record to Send Evidence";
                    }
                }
                else
                {
                    //error message time should pass 10:00
                    ViewBag.message = "Please Send the Evidence After 10:00 AM";
                }
            }
            else
            {
                //error message weekend
                ViewBag.message = "You are Not Allowed To send Evidence on A Weekend";
            }
            return View();
        }

        public ActionResult WarningList()
        {
            //context object
            ApplicationDbContext context = new ApplicationDbContext();

            //get parent info
            string pId = User.Identity.GetUserId().ToString();
            //string pId = "sdf";
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
    }
}
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
        public JsonResult GetEvents(DateTime start, DateTime end)
        {
            calanderEvents cv = new calanderEvents();
            ApplicationDbContext db = new ApplicationDbContext();
            List<calanderEvents> events = new List<calanderEvents>();
            string currentUserId = User.Identity.GetUserId();
            Models.Parent p = new Models.Parent();
            p = db.Parent.Where(r => r.parentId == currentUserId).FirstOrDefault();
            start = DateTime.Today;
            end = DateTime.Today;
           List<AbsenceRecord>  ab = new List<AbsenceRecord>();
            ab = db.AbsenceRecord.Where(a=>a.studentId==p.studentId).ToList();
            if (ab.Count() != 0) 
            {
                foreach(var g in ab)
                {
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
            }
           

            int x = 20;
            return Json(events, JsonRequestBehavior.AllowGet);
        }

    }
}
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

    }
}
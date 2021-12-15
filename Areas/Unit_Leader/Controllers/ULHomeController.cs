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
    }
}
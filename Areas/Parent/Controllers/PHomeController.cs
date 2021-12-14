using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LCCS_School_Parent_Communication_System.Areas.Homeroom_Teacher.Controllers
{
    [Authorize(Roles = "Homeroom_Teacher")]

    public class HTHomeController : Controller
    {
        // GET: Homeroom_Teacher/HTHome
        public ActionResult Index()
        {
            return View();
        }
    }
}
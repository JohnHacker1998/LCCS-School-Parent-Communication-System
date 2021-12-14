using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LCCS_School_Parent_Communication_System.Areas.Registrar.Controllers
{
    [Authorize(Roles = "Registrar")]

    public class RHomeController : Controller
    {
        // GET: Registrar/RHome
        public ActionResult Index()
        {
            return View();
        }
    }
}
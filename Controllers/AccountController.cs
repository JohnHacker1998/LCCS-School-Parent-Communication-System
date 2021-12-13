using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using LCCS_School_Parent_Communication_System.viewModels;
using LCCS_School_Parent_Communication_System.Identity;
using Microsoft.Owin.Security.Provider;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;

namespace LCCS_School_Parent_Communication_System.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel lv)
        {
            var appDbContext = new ApplicationDbContext();
            var userStore = new ApplicationUserStore(appDbContext);
            var userManager = new ApplicationUserManager(userStore);
            // var passwordH= Crypto.HashPassword(lv.password);
            var user = userManager.Find(lv.username, lv.password);
            if (ModelState.IsValid)
            {
                if (!userManager.IsInRole(user.Id, "AcademicDirector")){
                    return RedirectToAction("Index", "Default", new { area = "Academic_Director" });
                }
                else if (!userManager.IsInRole(user.Id, "UnitLeader"))
                {
                    return RedirectToAction("Index", "Default", new { area = "Unit_Leader" });
                }
                else if (!userManager.IsInRole(user.Id, "HomeroomTeacher"))
                {
                    return RedirectToAction("Index", "Default", new { area = "Homeroom_Teacher" });
                }
                else if (!userManager.IsInRole(user.Id, "Parent"))
                {
                    return RedirectToAction("Index", "Default", new { area = "Parent" });
                }
                else
                {
                    return RedirectToAction("Index", "Default", new { area = "Registrar" });
                }

            }
            else
            {
                ModelState.AddModelError("myerror", "Invalid username or password");
            }
            return View();
        }
    }
}
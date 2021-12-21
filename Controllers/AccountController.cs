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
            
            
            if (ModelState.IsValid)
            {
                var user = userManager.Find(lv.username, lv.password);

                int x = 0;
                if (user != null)
                {
                    var authenticationManager = HttpContext.GetOwinContext().Authentication;
                    var userIdentity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                    authenticationManager.SignIn(new AuthenticationProperties(), userIdentity);

                    if (userManager.IsInRole(user.Id, "AcademicDirector"))
                    {
                        return RedirectToAction("Index", "ADHome", new { area = "Academic_Director" });
                    }
                    else if (userManager.IsInRole(user.Id, "UnitLeader"))
                    {
                        return RedirectToAction("Index", "ULHome", new { area = "Unit_Leader" });
                    }
                    else if (userManager.IsInRole(user.Id, "HomeroomTeacher"))
                    {
                        return RedirectToAction("Index", "HTHome", new { area = "Homeroom_Teacher" });
                    }
                    else if (userManager.IsInRole(user.Id, "Parent"))
                    {
                        return RedirectToAction("Index", "PHome", new { area = "Parent" });
                    }
                    else if (userManager.IsInRole(user.Id, "Registrar"))
                    {
                        return RedirectToAction("Index", "RHome", new { area = "Registrar" });
                    }
                    else
                    {
                        return View();
                    }
                }

                ModelState.AddModelError("myerror", "Incorrect username or password.");
            }
            else
            {
                ModelState.AddModelError("myerror", "Invalid username or password");
            }
            return View();
        }
    }
}
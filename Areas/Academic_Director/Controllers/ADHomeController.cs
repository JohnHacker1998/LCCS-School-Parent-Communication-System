using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LCCS_School_Parent_Communication_System.viewModels;
using LCCS_School_Parent_Communication_System.Additional_Class;
using LCCS_School_Parent_Communication_System.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LCCS_School_Parent_Communication_System.Areas.Academic_Director.Controllers
{
  [Authorize(Roles = "AcademicDirector")]

    public class ADHomeController : Controller
    {
        // GET: Academic_Director/ADHome
        public ActionResult Index()
        {
            return View();
        }
       
        public ActionResult manageRegistrar()
        {
            var appDbContext = new ApplicationDbContext();
            
            var userStore = new ApplicationUserStore(appDbContext);
            var userManager = new ApplicationUserManager(userStore);
            ApplicationUser user = new ApplicationUser();
            RegistrarManagementViewModel rvm = new RegistrarManagementViewModel();

            List<ApplicationUser> users = new List<ApplicationUser>();
            users = appDbContext.Users.ToList();
            
            foreach(var k in users)
            {
                if (userManager.IsInRole(k.Id, "Registrar"))
                {
                    rvm.registrarList.Add(new ApplicationUser { 
                        Id=k.Id,
                        fullName= k.fullName,
                        UserName=k.UserName,
                        Email= k.Email 
                        
                    });
                }
                           
            }
         
            int x = 0; 
            return View(rvm);
        }
    
        [HttpPost]
        public ActionResult manageRegistrar(RegistrarManagementViewModel rmv){
            RegisterViewModel rv = new RegisterViewModel();
            Collection c = new Collection();
            rv.username = c.generateUserName();
            rv.password = c.generatePassword();
            rv.email = rmv.email;
            rv.fullName = rmv.fullName;
            c.RegisterUser(rv, "Registrar");
            string messageBody = "Registrar Account Username:" + rv.username + "Password=" + rv.password;
            //   c.sendMail(rv.username, messageBody);
            return View();
        }
        public async Task<ActionResult> deleteRegistrar(string theID)
        {
           // Collection collection = new Collection();
            int x = 0;
         //   collection.DeleteUser(theID);
            var appDbContext = new ApplicationDbContext();
            var userStore = new ApplicationUserStore(appDbContext);
            var userManager = new ApplicationUserManager(userStore);

            var user = await userManager.FindByIdAsync(theID);
            var result = await userManager.DeleteAsync(user);
            
            return RedirectToAction("Index");
        }

    }
}

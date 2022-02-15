using System;
using System.Threading.Tasks;
using LCCS_School_Parent_Communication_System.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.VisualBasic.ApplicationServices;

using Owin;

[assembly: OwinStartup(typeof(LCCS_School_Parent_Communication_System.Startup1))]

namespace LCCS_School_Parent_Communication_System
{
    public class Startup1
    {
        internal static IDataProtectionProvider DataProtectionProvider { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions() { AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie, LoginPath = new PathString("/Account/Login") });
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            this.CreateRolesAndUsers();

            DataProtectionProvider = app.GetDataProtectionProvider();
        }

       

       
        public void CreateRolesAndUsers()
        {
            
            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var appDbContext = new ApplicationDbContext();
            var appUserStore = new ApplicationUserStore(appDbContext);
            var userManager = new ApplicationUserManager(appUserStore);


            //Create AcademicDirector Role
            if (!roleManager.RoleExists("AcademicDirector"))
            {
                var role = new IdentityRole();
                role.Name = "AcademicDirector";
                roleManager.Create(role);
            }

            //Create Admin User
            if (userManager.FindByName("") == null)
            {
                var user = new ApplicationUser();
                user.UserName = "director";
                user.fullName = "Abebe kebede Ayele";
                user.Email= "lideta.catholic.cathedral@gmail.com";
                string userPassword = "director123";
               
                var chkUser = userManager.Create(user, userPassword);
                if (chkUser.Succeeded)
                {
                    userManager.AddToRole(user.Id, "AcademicDirector");
                    
                }
            }


            //Create Teacher Role
            if (!roleManager.RoleExists("Teacher"))
            {
                var role = new IdentityRole();
                role.Name = "Teacher";
                roleManager.Create(role);
            }

            //Create HomeRoom Role
            if (!roleManager.RoleExists("HomeRoom"))
            {
                var role = new IdentityRole();
                role.Name = "HomeRoom";
                roleManager.Create(role);
            }

            //Create UnitLeader Role
            if (!roleManager.RoleExists("UnitLeader"))
            {
                var role = new IdentityRole();
                role.Name = "UnitLeader";
                roleManager.Create(role);
            }

            //Create Student Role
            if (!roleManager.RoleExists("Registrar"))
            {
                var role = new IdentityRole();
                role.Name = "Registrar";
                roleManager.Create(role);
            }

            //Create Student Role
            if (!roleManager.RoleExists("Parent"))
            {
                var role = new IdentityRole();
                role.Name = "Parent";
                roleManager.Create(role);
            }

        }
    }
}

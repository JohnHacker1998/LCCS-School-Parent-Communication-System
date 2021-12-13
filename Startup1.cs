using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(LCCS_School_Parent_Communication_System.Startup1))]

namespace LCCS_School_Parent_Communication_System
{
    public class Startup1
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
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
            if (!roleManager.RoleExists("Student"))
            {
                var role = new IdentityRole();
                role.Name = "Student";
                roleManager.Create(role);
            }
        }
    }
}

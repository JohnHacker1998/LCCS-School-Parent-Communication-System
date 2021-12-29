using LCCS_School_Parent_Communication_System.Identity;
using LCCS_School_Parent_Communication_System.viewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace LCCS_School_Parent_Communication_System.Additional_Class
{
    public class RegistrarMethod
    {

        //function to populate section list in the UI
        public List<string> populateSection()
        {
            //basic objects
            ApplicationDbContext context = new ApplicationDbContext();
            List<string> section = new List<string>();

            //get all academic years
            var academicYears = context.AcademicYear.ToList();
            foreach (var getActive in academicYears)
            {
                //get start and end dates to check if today is in the middle
                if (!(DateTime.Compare(DateTime.Now, getActive.durationStart) < 0 || DateTime.Compare(DateTime.Now, getActive.durationEnd) > 0))
                {
                    //get sections on a given academic year
                    var getSection = context.Section.Where(s => s.academicYearId == getActive.academicYearName).ToList();

                    //check if sections exist in the academic year
                    if (getSection.Count != 0)
                    {
                        foreach (var getSectionName in getSection)
                        {
                            //populate section list
                            section.Add(getSectionName.sectionName);

                        }
                    }
                }
            }

            return section;
        }
        public String RegisterParent(RegisterViewModel registerViewModel, String role)
        {
            //basic objects for creation of user
            var appDbContext = new ApplicationDbContext();
            var userStore = new ApplicationUserStore(appDbContext);
            var userManager = new ApplicationUserManager(userStore);

            //hash password to store on the table
            var passwordHash = Crypto.HashPassword(registerViewModel.password);

            //create Application user object
            var user = new ApplicationUser()
            {
                UserName = registerViewModel.username,
                PasswordHash = passwordHash,
                fullName = registerViewModel.fullName,
                Email = registerViewModel.email,
                PhoneNumber=registerViewModel.phoneNumber,
                

            };

            //create user
            IdentityResult result = userManager.Create(user);

            //add asssociated role to the user
            if (result.Succeeded)
            {
                userManager.AddToRole(user.Id, role);
                return user.Id;
            }

            return null;
        }
    }
}
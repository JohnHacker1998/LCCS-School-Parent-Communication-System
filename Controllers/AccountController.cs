using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using LCCS_School_Parent_Communication_System.viewModels;
using LCCS_School_Parent_Communication_System.Identity;

using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;

using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.VisualBasic.ApplicationServices;
using LCCS_School_Parent_Communication_System.App_Start;
using Microsoft.Owin.Security.DataProtection;
using System.Net.Mail;
using System.Net;

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
                        return RedirectToAction("manageRegistrar", "ADHome", new { area = "Academic_Director" });
                    }
                    else if (userManager.IsInRole(user.Id, "UnitLeader"))
                    {
                        return RedirectToAction("LateComerManagement", "ULHome", new { area = "Unit_Leader" });
                    }
                    else if (userManager.IsInRole(user.Id, "HomeRoom"))
                    {
                        return RedirectToAction("addAttendance", "HTHome", new { area = "Homeroom_Teacher" });
                    }
                    else if (userManager.IsInRole(user.Id, "Parent"))
                    {
                        return RedirectToAction("viewAttendance", "PHome", new { area = "Parent" });
                    }
                    else if (userManager.IsInRole(user.Id, "Registrar"))
                    {
                        return RedirectToAction("parentManagement", "RHome", new { area = "Registrar" });
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

        public ActionResult profile()
        {
            //basic objects
            ApplicationDbContext context = new ApplicationDbContext();
            ProfileViewModel profileViewModel = new ProfileViewModel();

            //get current user id
            string uId = User.Identity.GetUserId().ToString();
            
            //get user information and populate to profileViewModel
            var user = context.Users.Find(uId);
            profileViewModel.fullName = user.fullName;
            profileViewModel.email = user.Email;
            profileViewModel.userName = user.UserName;

            return View(profileViewModel);
        }

        [HttpPost]
        public ActionResult profile(ProfileViewModel profileViewModel)
        {
            //var appDbContext = new ApplicationDbContext();
            //var userStore = new ApplicationUserStore(appDbContext);
            //var userManager = new ApplicationUserManager(userStore);

            //string uId = User.Identity.GetUserId().ToString();
            //var user = appDbContext.Users.Find(uId);

            //var checkName = appDbContext.Users.Where(u => u.UserName == profileViewModel.userName && u.Id != uId).FirstOrDefault();
            //if (checkName == null)
            //{
            //    user.UserName = profileViewModel.userName;
            //}

            ////var user = await UserManager.FindByIdAsync(id);
            ////var token = await UserManager.GeneratePasswordResetTokenAsync(user);
            ////var result = await UserManager.ResetPasswordAsync(user, token, "MyN3wP@ssw0rd");
            //var result = userManager.ChangePassword(uId, profileViewModel.password, profileViewModel.newPassword);

            //foreach(var error in result.Errors)
            //{
            //    ModelState.AddModelError("changepass", error);
            //}

           
            return View(profileViewModel);
        }

        public ActionResult EditFullName()
        {
            //basic objects
            ApplicationDbContext context = new ApplicationDbContext();
            ProfileEditFullName profileEditFullName = new ProfileEditFullName();

            //get current user id
            string uId = User.Identity.GetUserId().ToString();

            //populate the modal with the current fullName
            var user = context.Users.Find(uId);
            profileEditFullName.fullName = user.fullName;


            return PartialView("EditFullName", profileEditFullName);
        }

        [HttpPost]
        public ActionResult EditFullName(ProfileEditFullName profileEditFullName)
        {
            //context object
            ApplicationDbContext context = new ApplicationDbContext();

            //get current user id
            string uId = User.Identity.GetUserId().ToString();

            //check if the new fullName is already taken 
            var duplicate = context.Users.Where(u => u.fullName == profileEditFullName.fullName && u.Id != uId).FirstOrDefault();
            if (duplicate == null)
            {
                //update fullName
                var user = context.Users.Find(uId);
                user.fullName = profileEditFullName.fullName;
                int success=context.SaveChanges();

                if (success > 0)
                {
                    ViewBag.complete = "Full Name Updated Successfully";
                }
                else
                {
                    ViewBag.error = "Update Failed!!";
                }
            }
            else
            {
                ViewBag.error = "Name Taken By Another Account";
            }

            return PartialView("EditFullName", profileEditFullName);
        }



        public ActionResult EditUserName()
        {
            //basic objects
            ApplicationDbContext context = new ApplicationDbContext();
            ProfileEditUserName profileEditUserName = new ProfileEditUserName();

            //get current user id
            string uId = User.Identity.GetUserId().ToString();

            //populate the modal with the current username
            var user = context.Users.Find(uId);
            profileEditUserName.userName = user.UserName;


            return PartialView("EditUserName", profileEditUserName);
        }

        [HttpPost]
        public ActionResult EditUserName(ProfileEditUserName profileEditUserName)
        {
            //context object
            ApplicationDbContext context = new ApplicationDbContext();

            //get current user id
            string uId = User.Identity.GetUserId().ToString();

            //check if the new username is already taken 
            var duplicate = context.Users.Where(u => u.UserName == profileEditUserName.userName && u.Id != uId).FirstOrDefault();
            if (duplicate == null)
            {
                //update username
                var user = context.Users.Find(uId);
                user.UserName = profileEditUserName.userName;
                int success=context.SaveChanges();

                if (success > 0)
                {
                    ViewBag.complete = "UserName Updated Successfully";
                }
                else
                {
                    ViewBag.error = "Update Failed!!";
                }
            }
            else
            {
                ViewBag.error = "UserName Already Taken.";
            }

            return PartialView("EditUserName", profileEditUserName);
        }

        public ActionResult EditPassword()
        {
            return PartialView("EditPassword");
        }
        [HttpPost]
        public ActionResult EditPassword(ProfileEditPassword profileEditPassword)
        {
            //basic objects
            var appDbContext = new ApplicationDbContext();
            var userStore = new ApplicationUserStore(appDbContext);
            var userManager = new ApplicationUserManager(userStore);

            //get current user id
            string uId = User.Identity.GetUserId().ToString();

            //change password
            var result = userManager.ChangePassword(uId, profileEditPassword.oldPassword, profileEditPassword.newPassword);

            if (result.Succeeded)
            {
                ViewBag.complete = "Password Updated Successfully";
            }
            else
            {
                //display error message
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("changepass", error);
                }
            }
            return PartialView("EditPassword",profileEditPassword);
        }


        //[HttpPost]
        //public ActionResult edit(ProfileEditUserName profileEditUserName)
        //{

        //    return PartialView("edit", profileEditUserName);
        //}

        //public ActionResult edit()
        //{
        //    ProfileEditUserName profileEditUserName = new ProfileEditUserName();
        //    return PartialView("edit", profileEditUserName);
        //}


        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            var appDbContext = new ApplicationDbContext();
            //var userStore = new ApplicationUserStore(appDbContext);
            //var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();


            var user = appDbContext.Users.Where(u => u.Email == forgotPasswordViewModel.email).FirstOrDefault();


            //var provider = new DpapiDataProtectionProvider("YourAppName");
            //UserManager.UserTokenProvider = new DataProtectorTokenProvider<User, string>(provider.Create("UserToken"))
            //    as IUserTokenProvider<User, string>;

            //var userStore = new UserStore<IdentityUser>();
            //UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(userStore);
            //var user = userManager.FindByEmail(forgotPasswordViewModel.email);
            //var userManager = new ApplicationUserManager(userStore);
            //var provider = new DpapiDataProtectionProvider(System.Web.Hosting.HostingEnvironment.ApplicationHost.GetSiteName());
            //UserManager.UserTokenProvider = (IUserTokenProvider<IdentityUser, string>)(new DataProtectorTokenProvider<IdentityUser, string>(provider.Create("UserToken")) as IUserTokenProvider<IdentityUser, string>);
            //UserManager.PasswordResetTokens = new DataProtectorTokenProvider(Startup1.DataProtectionProvider.Create("PasswordReset"));
            
            var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("LCCS_School_Parent_Communication_System");
            
            UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            userManager.UserTokenProvider = new Microsoft.AspNet.Identity.Owin.DataProtectorTokenProvider<ApplicationUser>(provider.Create("PasswordReset"));


            string code = await userManager.GeneratePasswordResetTokenAsync(user.Id);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            await userManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");

            //create the mail message
            MailMessage message = new MailMessage("lideta.catholic.cathedral@gmail.com", user.Email);
            message.Subject = "Reset Password";
            message.Body = "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>";
            message.IsBodyHtml = true;

            //define the host and port number
            SmtpClient smtp = new SmtpClient();
            smtp.EnableSsl = true;
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;

            //send mail using sender credential
            NetworkCredential networkCredential = new NetworkCredential("lideta.catholic.cathedral@gmail.com", "Lccs@pi$s@65");
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = networkCredential;
            smtp.Send(message);


            //string code = userManager.GeneratePasswordResetToken(user.Id);
            //var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            //await userManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
            return RedirectToAction("Login", "Account");
        }

       

        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            var user = context.Users.Where(u => u.Email == resetPasswordViewModel.email).FirstOrDefault();

            var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("LCCS_School_Parent_Communication_System");
            userManager.UserTokenProvider = new Microsoft.AspNet.Identity.Owin.DataProtectorTokenProvider<ApplicationUser>(provider.Create("PasswordReset"));

            var result = await userManager.ResetPasswordAsync(user.Id, resetPasswordViewModel.code, resetPasswordViewModel.password);
            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Account");
            }
           
            return View();
        }

        

    }
}
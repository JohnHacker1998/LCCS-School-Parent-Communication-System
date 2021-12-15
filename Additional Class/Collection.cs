using LCCS_School_Parent_Communication_System.Identity;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Helpers;
using LCCS_School_Parent_Communication_System.viewModels;

namespace LCCS_School_Parent_Communication_System.Additional_Class
{
    public class Collection
    {
        //function for adding a user in to the identity
        public Boolean RegisterUser(RegisterViewModel registerViewModel,String role)
        {
            //basic objects for database and identity
            var appDbContext = new ApplicationDbContext();
            var userStore = new ApplicationUserStore(appDbContext);
            var userManager = new ApplicationUserManager(userStore);

            var passwordHash = Crypto.HashPassword(registerViewModel.password);

            //populate user information
            var user = new ApplicationUser()
            {
                UserName = registerViewModel.username,
                PasswordHash = passwordHash,
                fullName = registerViewModel.fullName,
                Email = registerViewModel.email
            };

            //create user
            IdentityResult result = userManager.Create(user);

            //add asssociated role to the user
            if (result.Succeeded)
            {
                userManager.AddToRole(user.Id,role);
                return true;
            }

                return false;
        }


        //function to generate random username 
        public String generateUserName()
        {
            //create pull of characters
            List<char> alphabet = new List<char>();
            for (char c = 'a'; c <= 'z'; c++)
            {
                alphabet.Add(c);
            }
            for (char c = 'A'; c <= 'Z'; c++)
            {
                alphabet.Add(c);
            }
            for (char c = '0'; c <= '9'; c++)
            {
                alphabet.Add(c);
            }

            //generate random username from pull of characters(lengeth 7)
            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();
            int j = 0;
            while (j <= 7)
            {
                sb.Append(alphabet[rnd.Next(0, alphabet.Count)]);
                j++;
            }
            string userName = sb.ToString();

            return userName;
        }

        //function to generate random password 
        public String generatePassword()
        {
            //create pull of characters
            List<char> alphabet = new List<char>();
            for (char c = 'a'; c <= 'z'; c++)
            {
                alphabet.Add(c);
            }
            for (char c = 'A'; c <= 'Z'; c++)
            {
                alphabet.Add(c);
            }
            for (char c = '0'; c <= '9'; c++)
            {
                alphabet.Add(c);
            }
            for (char c = '!'; c <= '~'; c++)
            {
                alphabet.Add(c);
            }

            //generate random username from pull of characters(lengeth 10)
            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();
            int j = 0;
            while (j <= 10)
            {
                sb.Append(alphabet[rnd.Next(0, alphabet.Count)]);
                j++;
            }
            string password = sb.ToString();

            return password;
        }

        //function to send email to specific user
        public void sendMail(string userName,string body)
        {
            //create the mail message(populate)
            MailMessage message = new MailMessage("lideta.catholic.cathedral@gmail.com", userName);
            message.Subject = "Lccs School-Parent Communication Credential";
            message.Body = body;
            message.IsBodyHtml = false;

            //define the host
            SmtpClient smtp = new SmtpClient();
            smtp.EnableSsl = true;
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;

            //send mail using sender credential
            NetworkCredential networkCredential = new NetworkCredential("lideta.catholic.cathedral@gmail.com", "Lccs@pi$s@65");
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = networkCredential;
            smtp.Send(message);
        }
        public async void DeleteUser(string id)
        {
            //basic objects for database and identity
            var appDbContext = new ApplicationDbContext();
            var userStore = new ApplicationUserStore(appDbContext);
            var userManager = new ApplicationUserManager(userStore);

            var user = await userManager.FindByIdAsync(id);
            var result = await userManager.DeleteAsync(user);


        }


    }
}
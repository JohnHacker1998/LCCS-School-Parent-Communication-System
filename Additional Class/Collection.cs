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
using System.Threading.Tasks;
using LCCS_School_Parent_Communication_System.Models;

namespace LCCS_School_Parent_Communication_System.Additional_Class
{
    public class Collection
    {
        //function for adding a user in to the identity
        public String RegisterUser(RegisterViewModel registerViewModel,String role)
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
                Email = registerViewModel.email
                
            };

            //create user
            IdentityResult result = userManager.Create(user);

            //add asssociated role to the user
            if (result.Succeeded)
            {
                userManager.AddToRole(user.Id,role);
                return user.Id;
            }

                return null;
        }


        //function to generate random username for new user 
        public String generateUserName()
        {
            //create poll of characters to be used by random function
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

            //generate random username from poll of characters(lengeth 7)
            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();

            //pick random character from alphabet and append to string builder
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
            //create poll of characters for creating random password
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

            //generate random password from the poll of characters(lengeth 10)
            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();

            //pick random character from alphabet and append to string builder
            int j = 0;
            while (j <= 10)
            {
                sb.Append(alphabet[rnd.Next(0, alphabet.Count)]);
                j++;
            }

            string password = sb.ToString();

            return password;
        }

        //function to send email to user
        public void sendMail(string email,string userName,string password)
        {
           
            //create the mail message
            MailMessage message = new MailMessage("lideta.catholic.cathedral@gmail.com", email);
            message.Subject = "Lccs School-Parent Communication Credential";
            message.Body = "Welcome to Lideta Catholic Catedral School.\nYour School-Parent Communication System account has been created.\n\n\tYour Username is: " + userName + "\n\tpassword is: " + password + "\n\nBe careful with the uppercase and lowercase, they matter"; ;
            message.IsBodyHtml = false;

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
        }

        //function to delete user account
        public async Task<string> DeleteUser(string id)
        {
            //basic objects for accessing information
            var appDbContext = new ApplicationDbContext();
            var userStore = new ApplicationUserStore(appDbContext);
            var userManager = new ApplicationUserManager(userStore);

            //delete user account using user id
            var user = await userManager.FindByIdAsync(id);
            var result = await userManager.DeleteAsync(user);

            //check if the deletion is successful
            if (result.Succeeded)
            {
                return "successful";
            }

            return "failed";
        }


        public string currentQuarter(int id)
        {
            //basic objects
            ApplicationDbContext context = new ApplicationDbContext();
            Student student = new Student();
            AcademicYear academicYear = new AcademicYear();
            string quarter = " ";

            //get academic year of the student
            student = context.Student.Where(s => s.studentId == id).FirstOrDefault();
            academicYear = context.AcademicYear.Where(a => a.academicYearName == student.academicYearId).FirstOrDefault();

            //check in which quarter is the current date
            if (DateTime.Compare(academicYear.quarterOneStart.Date,DateTime.Now.Date)<=0 && DateTime.Compare(academicYear.quarterOneEnd.Date,DateTime.Now.Date) >= 0)
            {
                quarter = academicYear.academicYearName + "-Q1";
            }
            else if(DateTime.Compare(academicYear.quarterTwoStart.Date, DateTime.Now.Date) <= 0 && DateTime.Compare(academicYear.quarterTwoEnd.Date, DateTime.Now.Date) >= 0)
            {
                quarter = academicYear.academicYearName + "-Q2";
            }
            else if(DateTime.Compare(academicYear.quarterThreeStart.Date,DateTime.Now.Date) <= 0 && DateTime.Compare(academicYear.quarterThreeEnd.Date, DateTime.Now.Date) >= 0)
            {
                quarter = academicYear.academicYearName + "-Q3";
            }
            else if(DateTime.Compare(academicYear.quarterFourStart.Date, DateTime.Now.Date) <= 0 && DateTime.Compare(academicYear.quarterFourEnd.Date, DateTime.Now.Date) >= 0)
            {
                quarter = academicYear.academicYearName + "-Q4";
            }

            return quarter;
        }



        public bool checkUserExistence(string email,string fullname)
        {
            var appDbContext = new ApplicationDbContext();

            var appuser = appDbContext.Users.Where(a => a.Email == email || a.fullName == fullname).FirstOrDefault();
            if (appuser == null)
            {
                return true;
            }
            return false;
        }
        

    }
}
using LCCS_School_Parent_Communication_System.Identity;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;

namespace LCCS_School_Parent_Communication_System.Additional_Class
{
    public class Collection
    {
        /* public Boolean RegisterUser(RegisterViewModel registerViewModel,String role)
        {
       

            var appDbContext = new ApplicationDbContext();
            var userStore = new ApplicationUserStore(appDbContext);
            var userManager = new ApplicationUserManager(userStore);

            var passwordHash = Crypto.HashPassword(registerViewModel.password);

            var user = new ApplicationUser()
            {
                UserName = registerViewModel.userName,
                PasswordHash = passwordHash,
                fullName= registerViewModel.fullName
            };

            IdentityResult result = userManager.Create(user);

            if (result.Succeeded)
            {
                userManager.AddToRole(user.Id,role);
                return true;
            }

                return false;
        }*/

        public String generateUserName()
        {
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

        public String generatePassword()
        {
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
        
}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using LCCS_School_Parent_Communication_System.Models;

namespace LCCS_School_Parent_Communication_System.viewModels
{
    public class parentViewModel
    {
        [Required(ErrorMessage = "Please enter Full Name")]
        [Display(Name = "Full Name")]
        [RegularExpression(@"^([a-z]\/|[A-Z]\/)?([a-z]|[A-Z]){2,}\s([a-z]\/|[A-Z]\/)?([a-z]|[A-Z]){2,}\s([a-z]\/|[A-Z]\/)?([a-z]|[A-Z]){2,}$", ErrorMessage = "Incorrect Full Name Format")]
        public String fullName { get; set; }
        [Required(ErrorMessage = "Email is Required")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Email is not valid.")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public String email { get; set; }
        [Required(ErrorMessage ="please enter phonenumber")]
        [Display(Name = "phoneNumber")]        
        [RegularExpression(@"[0-9]{4}[0-9]{6}", ErrorMessage = "Phone number isnot valid.")]
        public String phoneNumber { get; set; }
        [Required(ErrorMessage = "Please enter Full Name")]
        [Display(Name = "Student Full Name")]
        public String studentFullName { get; set; }
        
        public List<Student> studentList { get; set; }
        public List<Parent> parentList { get; set; }
        public int studentId { get; set; }

    }
}
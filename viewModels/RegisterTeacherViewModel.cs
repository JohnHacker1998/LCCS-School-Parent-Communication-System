using LCCS_School_Parent_Communication_System.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LCCS_School_Parent_Communication_System.viewModels
{
    public class RegisterTeacherViewModel
    {
        [Required(ErrorMessage = "Please enter Full Name")]
        [Display(Name = "Full Name")]
        public String fullName { get; set; }
        [Required(ErrorMessage = "Please enter Email")]
        [Display(Name = "Email")]
        public String email { get; set; }

        [Required(ErrorMessage = "Please enter Grade")]
        [Range(9, 12, ErrorMessage = "Please enter correct value")]
        [Display(Name = "Grade")]
        public int grade { get; set; }
        [Required(ErrorMessage = "Please enter Subject")]
        [Display(Name = "Subject")]
        public string subject { get; set; }

        public List<Teacher> teacherList { get; set; }
    }
}
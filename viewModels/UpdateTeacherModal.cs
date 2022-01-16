using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LCCS_School_Parent_Communication_System.viewModels
{
    public class UpdateTeacherModal
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Full Name is Required")]
        [Display(Name = "Full Name")]
        [RegularExpression(@"^([a-z]\/|[A-Z]\/)?([a-z]|[A-Z]){2,}\s([a-z]\/|[A-Z]\/)?([a-z]|[A-Z]){2,}\s([a-z]\/|[A-Z]\/)?([a-z]|[A-Z]){2,}$", ErrorMessage = "Incorrect Full Name Format")]
        public string fullName { get; set; }

        [Required(ErrorMessage = "Grade is Required")]
        [Range(9, 12, ErrorMessage = "Please Enter Grade in the Range of 9-12")]
        [Display(Name = "Grade")]
        public int grade { get; set; }

        [Required(ErrorMessage = "Subject is Required")]
        [Display(Name = "Subject")]
        public string subject { get; set; }
    }
}
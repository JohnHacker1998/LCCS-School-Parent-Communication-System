using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LCCS_School_Parent_Communication_System.viewModels
{
    public class AddGradeModal
    {
        //result,feedback,dates

        public int studentId { get; set; }

        [Required(ErrorMessage = "Date is Required")]
        [Display(Name = "Date")]
        public List<string> dates { get; set; }
        [Required(ErrorMessage = "Result is Required")]
        [Display(Name = "Result")]
        public int result { get; set; }

        [Display(Name = "Feedback")]
        public string feedback { get; set; }
    }
}
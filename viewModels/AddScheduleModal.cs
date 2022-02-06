using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LCCS_School_Parent_Communication_System.viewModels
{
    public class AddScheduleModal
    {

        public int scheduleId { get; set; }

        [Required(ErrorMessage = "Date is Required")]
        [Display(Name = "Date")]
        public string scheduleDate { get; set; }

        [Required(ErrorMessage = "Schedule For is Required")]
        [Display(Name = "Schedule For")]
        public List<string> scheduleFor { get; set; }

        [Required(ErrorMessage = "Subject is Required")]
        [Display(Name = "Subject")]
        public string subject { get; set; }

        [Required(ErrorMessage = "Grade is Required")]
        [Range(9, 12, ErrorMessage = "Please Enter Grade in the Range of 9-12")]
        [Display(Name = "Grade")]
        public int grade { get; set; }

        [Required(ErrorMessage = "Percentage is Required")]
        [Range(5, 100, ErrorMessage = "Please Enter Percentage in the Range of 5-100")]
        [Display(Name = "Percentage")]
        public int percentage { get; set; }
    }
}
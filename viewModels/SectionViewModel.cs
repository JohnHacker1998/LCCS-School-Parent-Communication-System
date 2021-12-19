using LCCS_School_Parent_Communication_System.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LCCS_School_Parent_Communication_System.viewModels
{
    public class SectionViewModel
    {
        [Required(ErrorMessage = "Please enter Grade")]
        [Range(9, 12, ErrorMessage = "Please enter correct value")]
        [Display(Name = "Grade")]
        public int grade { get; set; }
        [Required(ErrorMessage = "Section Letter is Required")]
        [Display(Name = "Section Letter")]
        public List<Char> letter { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "HomeRoom Teacher")]
        public List<string> teachers  { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Academic Year")]
        public List<string> academicYears { get; set; }
        
    }
}
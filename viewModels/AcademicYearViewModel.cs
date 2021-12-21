using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using LCCS_School_Parent_Communication_System.Models;

namespace LCCS_School_Parent_Communication_System.viewModels

{
    public class AcademicYearViewModel
    {
        public string yearStartTemp { get; set; }
      [Required(ErrorMessage ="Please fill the form.")]
    //[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
    [Display(Name ="Year Start")]
        public string yearStart { get; set; }

        [Required(ErrorMessage = "Please fill the form.")]
       
        //  [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Year End")]
        public string yearEnd { get; set; }

        [Required(ErrorMessage = "Please fill the form.")]
        [Display(Name = "Q1 Start")]
        //[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public string quarterOneStart { get; set; }

        [Required(ErrorMessage = "Please fill the form.")]
        [Display(Name = "Q1 Ending")]
        //[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public string quarterOneEnd { get; set; }

        [Required(ErrorMessage = "Please fill the form.")]
        [Display(Name = "Q2 Start")]
        //[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public string quarterTwoStart { get; set; }

        [Required(ErrorMessage = "Please fill the form.")]
        [Display(Name = "Q2 Ending")]
        //[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public string quarterTwoEnd { get; set; }

        [Required(ErrorMessage = "Please fill the form.")]
        [Display(Name ="Q3 Start")]
        // [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public string quarterThreeStart { get; set; }
        [Display(Name ="Q3 Ending")]

        [Required(ErrorMessage = "Please fill the form.")]
        //[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public string quarterThreeEnd { get; set; }
        

        [Required(ErrorMessage = "Please fill the form.")]
        [Display(Name = "Q4 Start")]
        //[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public string quarterFourStart { get; set; }
        [Required(ErrorMessage = "Please fill the form.")]
        [Display(Name = "Q4 Ending")]

        //  [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public string quarterFourEnd { get; set; }

        public List<AcademicYear> academicList { get; set; }

      

    }
}
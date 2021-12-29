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
      [Required(ErrorMessage ="Please fill Year Start.")]
    //[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
    [Display(Name ="Year Start")]
        public string yearStart { get; set; }

        [Required(ErrorMessage = "Please fill Year End.")]
       
        //  [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Year End")]
        public string yearEnd { get; set; }

        [Required(ErrorMessage = "Please fill Q1 Start.")]
        [Display(Name = "Q1 Start")]
        //[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public string quarterOneStart { get; set; }

        [Required(ErrorMessage = "Please fill Q1 End.")]
        [Display(Name = "Q1 Ending")]
        //[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public string quarterOneEnd { get; set; }

        [Required(ErrorMessage = "Please fill Q2 Start.")]
        [Display(Name = "Q2 Start")]
        //[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public string quarterTwoStart { get; set; }

        [Required(ErrorMessage = "Please fill Q2 End.")]
        [Display(Name = "Q2 Ending")]
        //[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public string quarterTwoEnd { get; set; }

        [Required(ErrorMessage = "Please fill Q3 Start.")]
        [Display(Name ="Q3 Start")]
        // [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        
        public string quarterThreeStart { get; set; }
        [Display(Name ="Q3 Ending")]

        [Required(ErrorMessage = "Please fill Q3 End.")]
        //[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public string quarterThreeEnd { get; set; }
        

        [Required(ErrorMessage = "Please fill Q4 Start.")]
        [Display(Name = "Q4 Start")]
        //[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public string quarterFourStart { get; set; }
        [Required(ErrorMessage = "Please fill Q4 End.")]
        [Display(Name = "Q4 Ending")]

        //  [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public string quarterFourEnd { get; set; }

        public List<AcademicYear> academicList { get; set; }

      

    }
}
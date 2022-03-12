using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LCCS_School_Parent_Communication_System.Models;
using System.ComponentModel.DataAnnotations;

namespace LCCS_School_Parent_Communication_System.viewModels
{
    public class groupStructureViewModel
    {
       
        [Display(Name = "Academic Quarter")]
        public string academicQuarter { get; set; }

        /* public int sectionName { get; set; }*/
        [Required(ErrorMessage = "Structure Name is required.")]
        [Display(Name = "Structure Name")]
        public string groupStructureName { get; set; }
        [Required(ErrorMessage = "Minimum Number of Members is required.")]
        [Display(Name = "Minimum Members")]

        [Range(2,100,
        ErrorMessage = "Value for {0} must be greater than {1}.")]
        public int minNumberOfMembers { get; set; }
        [Required(ErrorMessage = "Maximum Number of Members is required.")]
        [Display(Name = "Maximum Members")]
        [Range(2, 100,
        ErrorMessage = "Value for {0} must be greater than {1}.")]
        public int maxNumberOfMembers { get; set; }

        public List<GroupStructure> groupList { get; set; }
        [Display(Name ="Section List")]
        public List<String> sectionList { get; set; }
        public List<Student> studentList { get; set; }
    }
}
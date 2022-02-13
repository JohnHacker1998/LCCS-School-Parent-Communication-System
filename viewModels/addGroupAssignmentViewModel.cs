using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using LCCS_School_Parent_Communication_System.viewModels;
using LCCS_School_Parent_Communication_System.Models;

namespace LCCS_School_Parent_Communication_System.viewModels
{
    public class addGroupAssignmentViewModel
    {

        public int assignmentID { get; set; }
        public int sectionID { get; set; }
        public string yearlyQuarter { get; set; }

        [Required(ErrorMessage = "Please fill submission date.")]
        [Display(Name = "Submission Date")]
        public string submissionDate { get; set; }
        [Display(Name = "Document")]
        public string assignmentDocument { get; set; }

        public List<Assignment> listAssignment { get; set; }
         public List<String> sectionList { get; set; }
        public List<Student> studentList { get; set; }

        public string sectionName { get; set; }

        [Required(ErrorMessage = "Please fill assignment name.")]
        [Display(Name = "Assignment Name")]
        public string assignmentName { get; set; }
        [Required(ErrorMessage = "Please fill mark percentage.")]
        [Display(Name = "Mark Percentage")]
        [Range(1, 100,
        ErrorMessage = "Value for {0} must be greater than {1} and less than {2}.")]
        public int markPercentage { get; set; }

        public List<GroupStructure> groupStructureList { get; set; }
        [Display(Name = "Group List")]
        public List<Group> groupList { get; set; }

        public List<GroupStructure> gsList { get; set; }
        public int groupStructureId { get; set; }

    }
}
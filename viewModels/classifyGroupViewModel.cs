using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using LCCS_School_Parent_Communication_System.Models;

namespace LCCS_School_Parent_Communication_System.viewModels
{
    public class classifyGroupViewModel
    {
        [Required(ErrorMessage = "Group Name is required.")]
        [Display(Name = "Group Name")]
        public string groupName { get; set; }
        [Display(Name = "Minimum Members")]
        public int minMembers { get; set; }
        [Display(Name = "Maximum Members")]
        public int maxMembers { get; set; }
        
        public int groupStructureID { get; set; }
        [Display(Name = "Student List")]
        public List<Student> studentList { get; set; }

        public List<StudentGroupList> groupMembers { get; set; }

        public List<Group> groupList { get; set; }
    }
}
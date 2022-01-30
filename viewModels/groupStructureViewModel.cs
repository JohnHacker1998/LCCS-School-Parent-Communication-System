using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LCCS_School_Parent_Communication_System.Models;

namespace LCCS_School_Parent_Communication_System.viewModels
{
    public class groupStructureViewModel
    {
     public string academicQuarter { get; set; }

       /* public int sectionName { get; set; }*/
        public string groupStructureName { get; set; }
        public int minNumberOfMembers { get; set; }
        public int maxNumberOfMembers { get; set; }

        public List<GroupStructure> groupList { get; set; }
        public List<String> sectionList { get; set; }
        public List<Student> studentList { get; set; }
    }
}
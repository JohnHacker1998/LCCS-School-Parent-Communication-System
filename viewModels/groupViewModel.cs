using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LCCS_School_Parent_Communication_System.Models;

namespace LCCS_School_Parent_Communication_System.viewModels
{
    public class groupViewModel
    {


        public string groupName { get; set; }

        public int minMembers { get; set; }
        public int maxMembers { get; set; }
        public int groupStructureID { get; set; }
        public List<Student> studentList { get; set; }

        public List<StudentGroupList> groupMembers { get; set; }

        public List<Group> groupList { get; set; }
    }
}

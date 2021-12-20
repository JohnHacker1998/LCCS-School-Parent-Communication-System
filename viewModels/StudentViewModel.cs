using LCCS_School_Parent_Communication_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LCCS_School_Parent_Communication_System.viewModels
{
    public class StudentViewModel
    {
        public int Id { get; set; }
        public string fullName { get; set; }

        public List<string> sectionName { get; set; }

        public List<Student> student;
    }
}
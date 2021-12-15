using LCCS_School_Parent_Communication_System.Identity;
using LCCS_School_Parent_Communication_System.viewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LCCS_School_Parent_Communication_System.Models
{
    public class Teacher
    {

        [Key]
        [ForeignKey("ApplicationUser")]
        public string teacherId { get; set; }
        public int grade { get; set; }
        public string subject{ get; set; }

        public virtual ApplicationUser user { get; set; }
    }
}
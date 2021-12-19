using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LCCS_School_Parent_Communication_System.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LCCS_School_Parent_Communication_System.Models
{
    public class Parent
    {
        [ForeignKey("student")]
        public string studentId { get; set; }
        [Key]
        [ForeignKey("user")]
        public string parentId { get; set; }
        public virtual ApplicationUser user { get; set; }
        public virtual Student student { get; set; }
    }
}
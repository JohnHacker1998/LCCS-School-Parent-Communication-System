using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LCCS_School_Parent_Communication_System.Models
{
    public class StudentGroupList
    {
        [Key]
        [Column(Order =1)]
        [ForeignKey("student")]
        public int studentId { get; set; }
        [Key]
        [Column(Order = 2)]
        [ForeignKey("group")]
        public int groupId { get; set; }

        public virtual Student student { get; set; }
        public virtual Group group { get; set; }
    }
}
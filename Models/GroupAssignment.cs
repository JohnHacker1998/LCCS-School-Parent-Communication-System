using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LCCS_School_Parent_Communication_System.Models
{
    public class GroupAssignment
    {
        [Key]
        public int groupAssignmentId { get; set; }
     
        [ForeignKey("assignment")]
        public int assignmentId { get; set; }       
      
        public int grId { get; set; }

        public virtual Assignment assignment { get; set; }
       
    }
}
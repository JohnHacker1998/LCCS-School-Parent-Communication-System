using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LCCS_School_Parent_Communication_System.Models
{
    public class GroupStructureAssignment
    {
        [Key]
        public int groupStructureAssignmentId { get; set; }
        /*[Column(Order =1)]*/
        [ForeignKey("assignment")]
        public int assignmentId { get; set; }
        
        public int groupStructureId { get; set; }

        public virtual Assignment assignment { get; set; }
       
    }
}
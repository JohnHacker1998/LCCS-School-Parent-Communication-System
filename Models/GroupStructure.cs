using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LCCS_School_Parent_Communication_System.Models
{
    public class GroupStructure
    {
        [Key]
        public int groupStructureId { get; set; }
        public string academicQuarter { get; set; }
        [ForeignKey("section")]
        public int sectionId { get; set; }
        public string groupStructureName { get; set; }
        public int minNumberOfMembers { get; set; }
        public int maxNumberOfMembers { get; set; }

        public int completeStatus { get; set; }
        [ForeignKey("teacher")]
        public string teacherId { get; set; }

        public virtual Section section { get; set; }
        public virtual Teacher teacher { get; set; }
        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LCCS_School_Parent_Communication_System.Models
{
    public class Group
    {
        [Key]
        public int groupId { get; set; }
        [ForeignKey("groupStructure")]
        public int groupStrId{get; set;}

        public string groupName { get; set; }
        
        public virtual GroupStructure groupStructure { get; set; }
    }
}
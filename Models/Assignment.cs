using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LCCS_School_Parent_Communication_System.Models
{
    public class Assignment
    {
        [Key]
        public int assignmentId { get; set; }
        public DateTime datePosted { get; set; }

        public string yearlyQuarter { get; set; }
        public string assignmentType { get; set; }
        [ForeignKey("section")]
        public int sectionID { get; set; }
        public int numberOfMembers { get; set; }
       public DateTime submissionDate { get; set; }
        public byte[] assignmentDocument { get; set; }
        
        public string groupList { get; set; }
        public virtual Section section { get; set; }
       
    }
}
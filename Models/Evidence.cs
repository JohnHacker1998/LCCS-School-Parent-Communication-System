using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LCCS_School_Parent_Communication_System.Models
{
    public class Evidence
    {
        [Key]
        public int evidenceId { get; set; }
        [ForeignKey("parent")]
        public string parentId { get; set; }
        public string dateUpload { get; set; }

        public byte[] evidenceDocument { get; set; }
        public string approvalStatus { get; set; }
        public virtual Parent parent { get; set; }
    }
}
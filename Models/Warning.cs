using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LCCS_School_Parent_Communication_System.Models
{
    public class Warning
    {
        [Key]
        public int warningId { get; set; }
        [ForeignKey("student")]
        public int studentId { get; set; }

        public int grade { get; set; }

        public string warningType{ get; set;}

        public string WarningReadStatus { get; set; }

        public virtual Student student { get; set; }
    }
}
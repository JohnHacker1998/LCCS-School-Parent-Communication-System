using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LCCS_School_Parent_Communication_System.Models
{
    public class Suspension
    {
        [Key]
        public int suspensionID { get; set; }
        [ForeignKey("student")]
        public int studentId { get; set; }

        public DateTime startDate { get; set; }

        public DateTime endDate { get; set; }

        public virtual Student student { get; set; }

    }
}
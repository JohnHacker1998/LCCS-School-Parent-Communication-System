using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LCCS_School_Parent_Communication_System.Models
{
    public class LateComer
    {
        [Key]
        public int lateId { get; set; }
        [ForeignKey("student")]
        public int studentId { get; set; }

        public DateTime lateDate { get; set; }

        public string academicPeriod { get; set; }

        public int dayCount { get; set; }

        public virtual Student student { get; set; }
    }
}
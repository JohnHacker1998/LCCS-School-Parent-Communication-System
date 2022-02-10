using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LCCS_School_Parent_Communication_System.Models
{
    public class AbsenceRecord
    {
        [Key]        public int recordId { get; set; }

        public string academicPeriod { get; set; }        public DateTime recordDate { get; set; }        [ForeignKey("student")]        public int studentId { get; set; }        public int count { get; set; }                public string evidenceFlag { get; set; }        public virtual Student student { get; set; }
       
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LCCS_School_Parent_Communication_System.Models
{
    public class AbsenceRecord
    {
        [Key]        public int recordId { get; set; }

        public string academicPeriod { get; set; }        public string recordDate { get; set; }        [ForeignKey("student")]        public int studentId { get; set; }        public int count { get; set; }        public virtual Student student { get; set; }

    }
}
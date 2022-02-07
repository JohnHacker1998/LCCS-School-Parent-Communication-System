using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LCCS_School_Parent_Communication_System.Models
{
    public class Result
    {
        [Key]
        public int resultId { get; set; }
        [ForeignKey("teacher")]
        public string teacherId { get; set; }

        [ForeignKey("student")]
        public int studentId { get; set; }
        public int result { get; set; }
        public string feedback { get; set; }
        [ForeignKey("schedule")]
        public int? scheduleId { get; set; }
        [ForeignKey("assignment")]
        public int? assignmentId { get; set; }

        public string resultFor { get; set; }
        public int percent { get; set; }
        public string academicYear { get; set; }
        public int grade { get; set; }

        public virtual Teacher teacher { get; set; }
        public virtual Schedule schedule { get; set; }
        public virtual Assignment assignment { get; set; }
        public virtual Student student { get; set; }
    }
}
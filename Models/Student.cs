using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LCCS_School_Parent_Communication_System.Models
{
    public class Student
    {
        [Key]
        public int studentId { get; set; }

        public string fullName { get; set; }

        public string sectionName { get; set; }
        [ForeignKey("academicYear")]
        public string academicYearId { get; set; }
        public virtual AcademicYear academicYear { get; set; }
    }
}
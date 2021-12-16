using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LCCS_School_Parent_Communication_System.Models
{
    public class Section
    {

        [Key]       
        public int sectionId { get; set; }
        [ForeignKey("academicYear")]
        public string academicYearId { get; set; }
        [ForeignKey("teacher")]
        public string teacherId { get; set; }
        public string sectionName { get; set; }
        public virtual Teacher teacher { get; set; }
        public virtual AcademicYear academicYear { get; set; }

        public Section(int x)
        {
            teacher = new Teacher();
            academicYear = new AcademicYear();

        }
        public Section()
        {

        }
       

    }
}
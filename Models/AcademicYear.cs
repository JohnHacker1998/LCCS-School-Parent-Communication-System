using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LCCS_School_Parent_Communication_System.Models
{
    public class AcademicYear
    {
        [Key]
        public string academicYearName { get; set; }
        public DateTime quarterOneStart { get; set; }
        public DateTime quarterOneEnd { get; set; }
        public DateTime quarterTwoStart { get; set; }
        public DateTime quarterTwoEnd { get; set; }
        public DateTime quarterThreeStart { get; set; }
        public DateTime quarterThreeEnd { get; set; }
        public DateTime quarterFourStart { get; set; }
        public DateTime quarterFourEnd { get; set; }
        public DateTime academicDurationStart { get; set; }
        public DateTime academicDurationEnd { get; set; }

    }
}
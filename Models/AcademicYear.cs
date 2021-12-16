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
        public string quarterOne { get; set; }
        public string quarterTwo { get; set; }
        public string quarterThree { get; set; }
        public string quarterFour { get; set; }
        public string duration { get; set; }


    }
}
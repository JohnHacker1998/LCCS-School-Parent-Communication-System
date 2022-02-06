using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LCCS_School_Parent_Communication_System.Models
{
    public class Schedule
    {
        [Key]
        public int scheduleId { get; set; }

        public DateTime scheduleDate { get; set; }

        public string scheduleFor { get; set; }

        public string subject { get; set; }
        public int grade { get; set; }

        public int percentage { get; set; }

        public string academicYear{ get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using LCCS_School_Parent_Communication_System.Models;

namespace LCCS_School_Parent_Communication_System.viewModels
{
    public class AbsenceRecordViewMoel
    {
        public int recordId { get; set; }

        [Required(ErrorMessage = "Please insert academic period")]

        public string academicPeriod { get; set; }


        public DateTime recordDate { get; set; }

        public int studentId { get; set; }
        public int[] selectedStudents { get; set; }
        public IEnumerable<SelectListItem> studentList { get; set; }
        public List<AbsenceRecord> absenceList { get; set; }


       
    }
}
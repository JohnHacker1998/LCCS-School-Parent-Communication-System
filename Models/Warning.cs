using LCCS_School_Parent_Communication_System.Additional_Class;
using LCCS_School_Parent_Communication_System.Identity;
using LCCS_School_Parent_Communication_System.viewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNet.Identity;
using System.Web;

namespace LCCS_School_Parent_Communication_System.Models
{
    public class Warning
    {
        [Key]
        public int warningId { get; set; }

        public DateTime warningDate { get; set; }
        [ForeignKey("student")]
        public int studentId { get; set; }

        public int grade { get; set; }

        public string warningType { get; set; }
        public string academicYear { get; set; }

        public string WarningReadStatus { get; set; }

        public virtual Student student { get; set; }

        public Warning()
        {
            Student student = new Student();
        }
    }
}
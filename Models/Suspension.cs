﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LCCS_School_Parent_Communication_System.Models
{
    public class Suspension
    {
        [Key]
        public int suspensionID { get; set; }
        [ForeignKey("student")]
        public int studentId{ get; set; }

        public DateTime startDate { get; set; }

        public DateTime endDate { get; set; }

        public virtual Student student { get; set; }

    }
}
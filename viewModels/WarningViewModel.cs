﻿using LCCS_School_Parent_Communication_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LCCS_School_Parent_Communication_System.viewModels
{
    public class WarningViewModel
    {
        public List<Student> eligible { get; set; }
        public List<Student> nonViewed { get; set; }
    }
}
using LCCS_School_Parent_Communication_System.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LCCS_School_Parent_Communication_System.viewModels
{
    public class LateComerViewModel
    {
        
        public int ID { get; set; }

        public List<Student> students { get; set; }
    }
}
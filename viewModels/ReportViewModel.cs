using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LCCS_School_Parent_Communication_System.viewModels
{
    public class ReportViewModel
    {
        public List<string> subject { get; set; }

        public List<int> score { get; set; }
        public List<int> outOf { get; set; }

        public int totalAssesment { get; set; }
        public int completeAssesment { get; set; }
        public int incompleteAssesment { get; set; }
        public int reassesement { get; set; }

        public int absentDays { get; set; }

        public int reasonable { get; set; }
        public int nonReasonable { get; set; }
        public int lateDays { get; set; }
    }
}
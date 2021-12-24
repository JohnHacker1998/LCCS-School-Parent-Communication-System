using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LCCS_School_Parent_Communication_System.viewModels
{
    public class EvidenceApprovalViewModel
    {
        public int Id { get; set; }
        public List<string> days { get; set; }
        public byte[] document { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LCCS_School_Parent_Communication_System.Models;

namespace LCCS_School_Parent_Communication_System.viewModels
{
    public class announcementViewModel
    {
        public int announcementID { get; set; }

        public string announcementTitle { get; set; }

        public string announcementContent { get; set; }

        public byte[] announcementDocument { get; set; }

        public string announcementType { get; set; }

        public DateTime endDate { get; set; }

        public DateTime postDate { get; set; }

        public List<Section> sectionList { get; set; }

        public List<Announcement> announcementList { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace LCCS_School_Parent_Communication_System.Models
{
    public class Announcement
    {
        [Key]
        public int announcementID { get; set; }

        public string announcementTitle { get; set; }

        public string announcementContent { get; set; }

        public byte[] announcementDocument { get; set; }

        public string announcementType { get; set; }
        
        public DateTime endDate { get; set; }
        
        public DateTime postDate { get; set; }
        public string filName { get; set; }
        public int updateStatus { get; set; }

        public int viewedStatus { get; set; }


    }
}
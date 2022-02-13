using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LCCS_School_Parent_Communication_System.Models;

namespace LCCS_School_Parent_Communication_System.Models
{
    public class studentAnnouncement
    {

        [Key]
        [ForeignKey("announcement")]
        [Column(Order =1)]
        
        public int announcementId { get; set; }
        [Key]
        [ForeignKey("student")]
        [Column(Order =2)]
        public int studentId { get; set; }

        public virtual Student student{get;set;}
        public virtual Announcement announcement { get; set; }
    }
}
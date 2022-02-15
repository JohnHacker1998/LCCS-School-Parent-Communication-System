﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LCCS_School_Parent_Communication_System.Models
{
    public class sectionAnnouncement
    {

        [Key]
        [ForeignKey("announcement")]
        [Column(Order =1)]

        public int announcementId { get; set; }
        [Key]
        [ForeignKey("section")]
        [Column(Order = 2)]
        public int sectionId { get; set; }

        public virtual Section section { get; set; }
        public virtual Announcement announcement { get; set; }
    }
}
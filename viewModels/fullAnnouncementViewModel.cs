using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LCCS_School_Parent_Communication_System.Models;
using LCCS_School_Parent_Communication_System.viewModels;
using System.ComponentModel.DataAnnotations;

namespace LCCS_School_Parent_Communication_System.viewModels
{
    public class fullAnnouncementViewModel
    {

        [Required(ErrorMessage = "Announcement title is required.")]
        [Display(Name = "Title")]
        public string announcementTitle { get; set; }

        [Required(ErrorMessage = "Announcement content is required.")]
        [Display(Name = "Content")]

        public string announcementContent { get; set; }

        [Required(ErrorMessage = "End Date is required.")]
        [Display(Name = "End Date")]

        public string endDate { get; set; }

        public List<Announcement> announcementList { get; set; }
        public List<gradeViewModel> gradeList { get; set; }

      
        [Display(Name = "Document")]
        public string announcementDocument { get; set; }
    }
}
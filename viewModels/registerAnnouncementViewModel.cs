using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LCCS_School_Parent_Communication_System.viewModels;
using LCCS_School_Parent_Communication_System.Models;
using System.ComponentModel.DataAnnotations;

namespace LCCS_School_Parent_Communication_System.viewModels
{
    public class registerAnnouncementViewModel
    {
        [Required(ErrorMessage = "Announcement title is required.")]
        [Display(Name = "Title")]
        public string announcementTitle { get; set; }
        [Required(ErrorMessage = "Announcement content is required.")]
        [Display(Name = "Content")]
        public string announcementContent { get; set; }
        
        [Display(Name = "Document")]
        public string announcementDocument { get; set; }
        [Required(ErrorMessage = "End date is required.")]
        [Display(Name = "End Date")]
        public string endDate { get; set; }
        public int grade { get; set; }

      [Display(Name ="Section List")]
        public List<Section> sectionList { get; set; }

        public List<Announcement> announcementList { get; set; }
        public List<gradeViewModel> gradeList { get; set; }

        public List<Student> studentList { get; set; }
    
        public string academicYear { get; set; }
        public int studentId { get; set; }
        public string studentName { get; set; }
    }
}
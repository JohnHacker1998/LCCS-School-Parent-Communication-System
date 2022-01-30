using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LCCS_School_Parent_Communication_System.viewModels;
using LCCS_School_Parent_Communication_System.Models;
using LCCS_School_Parent_Communication_System.Identity;
using Microsoft.AspNet.Identity;
using LCCS_School_Parent_Communication_System.Additional_Class;

namespace LCCS_School_Parent_Communication_System.Areas.Teacher.Controllers
{

    
    public class THomeController : Controller
    {
        // GET: Teacher/THome
        public ActionResult Index()
        {
            return View();
        }
      /*  public ActionResult modalAddAssignment()
        {
            
            string currentID = User.Identity.GetUserId();
            Models.Teacher t = new Models.Teacher();
            HomeroomTeacherMethod ht = new HomeroomTeacherMethod();
            AcademicYear temp = new AcademicYear();
            ApplicationDbContext db = new ApplicationDbContext();
            assignmentViewModel avm = new assignmentViewModel();
            avm.studentList = new List<Student>();

            List<Section> s = new List<Section>();
            avm.listAssignment = new List<Assignment>();
            avm.listAssignment = db.Assignment.ToList();
            avm.assignmentType = new List<String>();
            avm.assignmentType.Add("Individual");
            avm.assignmentType.Add("Group");
            avm.sectionList = new List<string>();
            t = db.Teacher.Where(a => a.teacherId == currentID).FirstOrDefault();
            string teacherGrade = t.grade.ToString();
            s = db.Section.Where(a => a.sectionName.StartsWith(teacherGrade)).ToList();
            string currentAcademicYearName = "";
            foreach (var k in s)
            {
                if (ht.isInAcademicYear(k.academicYearId))
                {
                    currentAcademicYearName = k.academicYearId;
                    break;
                }
            }
            temp = db.AcademicYear.Where(a => a.academicYearName == currentAcademicYearName).FirstOrDefault();
            string quarter = ht.whichQuarter(currentAcademicYearName);
            var currentSections = db.Section.Where(a => a.academicYearId == currentAcademicYearName);
            avm.studentList = db.Student.Where(a => a.academicYearId == currentAcademicYearName).ToList();
            foreach (var k in currentSections)
            {
                avm.sectionList.Add(k.sectionName);
            }
            
            return PartialView(avm);
        }
        [HttpPost]
        public ActionResult modalAddAssignment(assignmentViewModel avm, string file, string assignmentType, string sectionName)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            HomeroomTeacherMethod ht = new HomeroomTeacherMethod();
            Assignment a = new Assignment();
            ViewBag.selectedType = " ";
            ViewBag.selectedSection = " ";
            ViewBag.successfulMessage = " ";
            ViewBag.failedMessage = " ";
            int currentSectionID = 0;

            var currentSection = new List<Section>();
            currentSection = db.Section.Where(g => g.sectionName == sectionName).ToList();
            foreach (var k in currentSection)
            {
                if (ht.isInAcademicYear(k.academicYearId))
                {
                    currentSectionID = k.sectionId;
                    break;
                }
            }
            var theSection = db.Section.Where(g => g.sectionId == currentSectionID).FirstOrDefault();
            string currentQuarter = ht.whichQuarter(theSection.academicYearId);

            if (currentSectionID != 0 && assignmentType != null && ht.beforeQuarterEnd(Convert.ToDateTime(avm.submissionDate), theSection.academicYearId) && (currentQuarter == "Q1" || currentQuarter == "Q2" || currentQuarter == "Q3" || currentQuarter == "Q4"))
            {
                a.sectionID = currentSectionID;
                a.assignmentType = assignmentType;
                if (assignmentType == "Group" && avm.numberOfGroupMembers >= 2)
                    a.numberOfMembers = avm.numberOfGroupMembers;
                a.datePosted = DateTime.Now.Date;
                a.submissionDate = Convert.ToDateTime(avm.submissionDate).Date;
                a.yearlyQuarter = theSection.academicYearId + "-" + currentQuarter;
                int length = file.Length;
                byte[] upload = new byte[length];
                //file.InputStream.Read(upload, 0, length);
                a.assignmentDocument = upload;
                db.Assignment.Add(a);
                db.SaveChanges();
                ViewBag.successfulMessage = "Assignment added Successfully.";
            }
            else
            {
                ViewBag.failedMessage = "Invalid Assignment";
            }


            return PartialView("addAssignment");
        }*/
      public ActionResult addAssignment()
        {
            string currentID = User.Identity.GetUserId();
            Models.Teacher t = new Models.Teacher();
            HomeroomTeacherMethod ht = new HomeroomTeacherMethod();
            AcademicYear temp = new AcademicYear();
            ApplicationDbContext db = new ApplicationDbContext();
            assignmentViewModel avm = new assignmentViewModel();
            avm.studentList = new List<Student>();

            List<Section> s = new List<Section>();
            avm.listAssignment = new List<Assignment>();
            avm.listAssignment = db.Assignment.ToList();
            avm.assignmentType = new List<String>();
            avm.assignmentType.Add("Individual");
            avm.assignmentType.Add("Group");
            avm.sectionList = new List<string>();
            t = db.Teacher.Where(a => a.teacherId == currentID).FirstOrDefault();
            string teacherGrade = t.grade.ToString();
            s = db.Section.Where(a => a.sectionName.StartsWith(teacherGrade)).ToList();
            string currentAcademicYearName = "";
            foreach (var k in s)
            {
                if (ht.isInAcademicYear(k.academicYearId))
                {
                    currentAcademicYearName = k.academicYearId;
                    break;
                }
            }
            temp = db.AcademicYear.Where(a => a.academicYearName == currentAcademicYearName).FirstOrDefault();
            string quarter = ht.whichQuarter(currentAcademicYearName);
            var currentSections = db.Section.Where(a => a.academicYearId == currentAcademicYearName);
            avm.studentList = db.Student.Where(a => a.academicYearId == currentAcademicYearName).ToList();
            foreach (var k in currentSections)
            {
                avm.sectionList.Add(k.sectionName);
            }

            return View(avm);
        }
        public bool fulfillsPercentage(string yearlyQuarter,int sectionID,string teacherID,int percentage)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<Assignment> assignmentList = new List<Assignment>();
            assignmentList = db.Assignment.Where(a => a.yearlyQuarter == yearlyQuarter && a.sectionID == sectionID && a.teacherId == teacherID).ToList();
            int sum = 0;
            foreach(var k in assignmentList)
            {
                sum += k.markPercentage;
            }

            if (sum+percentage<=15)
            {
                return true;
            }
            return false;
        }
        [HttpPost]
        public ActionResult addAssignment(assignmentViewModel avm, HttpPostedFileBase file, string assignmentType, string sectionName)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            HomeroomTeacherMethod ht = new HomeroomTeacherMethod();
            Assignment a = new Assignment();
            Models.Teacher t = new Models.Teacher();
 
            AcademicYear temp = new AcademicYear();
            avm.studentList = new List<Student>();
            avm.listAssignment = new List<Assignment>();
           

            List<Section> s = new List<Section>();


            ViewBag.selectedType = " ";
            ViewBag.selectedSection = " ";
            ViewBag.successfulMessage = " ";
            ViewBag.failedMessage = " ";
            ViewBag.doesntFulfillMessage = " ";
            int currentSectionID = 0;
            string currentTeacherID = User.Identity.GetUserId();

            var currentSection = new List<Section>();
            currentSection = db.Section.Where(g => g.sectionName == sectionName).ToList();
            foreach (var k in currentSection)
            {
                if (ht.isInAcademicYear(k.academicYearId))
                {
                    currentSectionID = k.sectionId;
                    break;
                }
            }
            var theSection = db.Section.Where(g => g.sectionId == currentSectionID).FirstOrDefault();
            string currentQuarter = ht.whichQuarter(theSection.academicYearId);
            
            if (currentSectionID != 0 && assignmentType != null && ht.beforeQuarterEnd(Convert.ToDateTime(avm.submissionDate), theSection.academicYearId) && (currentQuarter == "Q1" || currentQuarter == "Q2" || currentQuarter == "Q3" || currentQuarter == "Q4") && fulfillsPercentage(theSection.academicYearId + "-" + currentQuarter, currentSectionID, currentTeacherID,avm.markPercentage))
            {
                a.sectionID = currentSectionID;
                a.assignmentType = assignmentType;
                if (assignmentType == "Group" && avm.numberOfGroupMembers >= 2)
                    a.numberOfMembers = avm.numberOfGroupMembers;
                a.datePosted = DateTime.Now.Date;
                a.submissionDate = Convert.ToDateTime(avm.submissionDate).Date;
                a.yearlyQuarter = theSection.academicYearId + "-" + currentQuarter;
                int length = file.ContentLength;
                byte[] upload = new byte[length];
                file.InputStream.Read(upload, 0, length);
                a.assignmentDocument = upload;
                a.assignmentName = avm.assignmentName;              
                a.markPercentage = avm.markPercentage;
                a.teacherId = currentTeacherID;
                db.Assignment.Add(a);
                db.SaveChanges();
                ViewBag.successfulMessage = "Assignment added Successfully.";
                avm.listAssignment = db.Assignment.Where(x => x.teacherId == currentTeacherID && x.yearlyQuarter == a.yearlyQuarter).ToList();
                return View("assignmentManagement", avm);
            }
            else
            {
                if(currentQuarter=="no")
                {
                    ViewBag.failedMessage = "Quarter doesn't exist";
                    
                }
                if (currentQuarter == "gap")
                {
                    ViewBag.failedMessage = "Current timespan is a gap";
                }
                if(fulfillsPercentage(theSection.academicYearId + "-" + currentQuarter, currentSectionID, currentTeacherID,avm.markPercentage) == false)
                {
                    ViewBag.doesntFulfillMessage="You have already reached the maximum assignment Mark Percentage.";
                }
               
            }
            avm.assignmentType = new List<String>();
            avm.assignmentType.Add("Individual");
            avm.assignmentType.Add("Group");
            avm.sectionList = new List<string>();
            t = db.Teacher.Where(ab => ab.teacherId == currentTeacherID).FirstOrDefault();
            string teacherGrade = t.grade.ToString();
            s = db.Section.Where(ab => ab.sectionName.StartsWith(teacherGrade)).ToList();
            string currentAcademicYearName = "";
            foreach (var k in s)
            {
                if (ht.isInAcademicYear(k.academicYearId))
                {
                    currentAcademicYearName = k.academicYearId;
                    break;
                }
            }
            temp = db.AcademicYear.Where(ab => ab.academicYearName == currentAcademicYearName).FirstOrDefault();
            string quarter = ht.whichQuarter(currentAcademicYearName);
            var currentSections = db.Section.Where(ab => ab.academicYearId == currentAcademicYearName);
            avm.studentList = db.Student.Where(ab => ab.academicYearId == currentAcademicYearName).ToList();
            foreach (var k in currentSections)
            {
                avm.sectionList.Add(k.sectionName);
            }

            return View(avm);

        }
        public ActionResult assignmentManagement()
        {
            ViewBag.selectedType = " ";
            ViewBag.selectedSection = " ";
            string currentID=User.Identity.GetUserId();
            Models.Teacher t = new Models.Teacher();
            HomeroomTeacherMethod ht = new HomeroomTeacherMethod();
            AcademicYear temp = new AcademicYear();
            ApplicationDbContext db = new ApplicationDbContext();
            assignmentViewModel avm = new assignmentViewModel();
            avm.studentList = new List<Student>();
            
            List<Section> s = new List<Section>();
            avm.listAssignment = new List<Assignment>();
            avm.listAssignment = db.Assignment.ToList();
            avm.assignmentType = new List<String>();
            avm.assignmentType.Add("Individual");
            avm.assignmentType.Add("Group");
            avm.sectionList = new List<string>();
            t = db.Teacher.Where(a => a.teacherId == currentID).FirstOrDefault();
            string teacherGrade = t.grade.ToString();
            s = db.Section.Where(a => a.sectionName.StartsWith(teacherGrade)).ToList();
            string currentAcademicYearName = "";
            foreach(var k in s)
            {
                if (ht.isInAcademicYear(k.academicYearId))
                {
                    currentAcademicYearName = k.academicYearId;
                    break;
                }
            }
            temp = db.AcademicYear.Where(a => a.academicYearName == currentAcademicYearName).FirstOrDefault();
            string quarter = ht.whichQuarter(currentAcademicYearName);
            var currentSections = db.Section.Where(a => a.academicYearId == currentAcademicYearName);
            avm.studentList = db.Student.Where(a => a.academicYearId == currentAcademicYearName).ToList();
            foreach(var k in currentSections)
            {
                avm.sectionList.Add(k.sectionName);
            }
            int c = avm.studentList.Count();
            avm.studentArray = new string[c * 2];
            int st = 0;
            /*   foreach(var k in avm.studentList)
               {
                  if(st<avm.studentArray.Length)
                   { 
                   avm.studentArray[st] = k.sectionName;
                   avm.studentArray[st + 1] = k.fullName;

                   }
                   st++;
               }*/
            int y = 0;
            for(int x = 0; x < avm.studentList.Count(); x++)
            {
               
                avm.studentArray[y] = avm.studentList[x].sectionName;
                y++;
                avm.studentArray[y] = avm.studentList[x].fullName;
                y++;
            }
            return View(avm);
        }

        [HttpPost]
        public ActionResult assignmentManagement(assignmentViewModel avm,string add, HttpPostedFileBase file, string assignmentType,string sectionName,string select,string delete,string assId,string update, string btnSubmitGrouping, string txtValue)
        {
            ViewBag.selectedType = " ";
            ViewBag.selectedSection = " ";
            ViewBag.successfulMessage = " ";
       
            ViewBag.failedMessage = " ";
            string currentID = User.Identity.GetUserId();
            ApplicationDbContext db = new ApplicationDbContext();
            Assignment a = new Assignment();
            HomeroomTeacherMethod ht = new HomeroomTeacherMethod();
            AcademicYear ay = new AcademicYear();
            AcademicYear temp = new AcademicYear();
            avm.studentList = new List<Student>();
            
            int currentSectionID = 0;
            if (add != null)
            {
                var currentSection = new List<Section>();
                currentSection = db.Section.Where(g => g.sectionName == sectionName).ToList();
                foreach (var k in currentSection)
                {
                    if (ht.isInAcademicYear(k.academicYearId))
                    {
                        currentSectionID = k.sectionId;
                        break;
                    }
                }
                var theSection = db.Section.Where(g => g.sectionId == currentSectionID).FirstOrDefault();
                string currentQuarter = ht.whichQuarter(theSection.academicYearId);

                if (currentSectionID != 0 && assignmentType != null && ht.beforeQuarterEnd(Convert.ToDateTime(avm.submissionDate), theSection.academicYearId) && (currentQuarter == "Q1" || currentQuarter == "Q2" || currentQuarter == "Q3" || currentQuarter == "Q4"))
                {
                    a.sectionID = currentSectionID;
                    a.assignmentType = assignmentType;
                    if (assignmentType == "Group" && avm.numberOfGroupMembers >= 2)
                        a.numberOfMembers = avm.numberOfGroupMembers;
                    a.datePosted = DateTime.Now.Date;
                    a.submissionDate = Convert.ToDateTime(avm.submissionDate).Date;
                    a.yearlyQuarter = theSection.academicYearId + "-" + currentQuarter;
                    int length = file.ContentLength;
                    byte[] upload = new byte[length];
                    file.InputStream.Read(upload, 0, length);
                    a.assignmentDocument = upload;
                    db.Assignment.Add(a);
                    db.SaveChanges();
                    ViewBag.successfulMessage = "Assignment added Successfully.";
                }
                else
                {
                    ViewBag.failedMessage = "Invalid Assignment";
                }




            }
            else if (select != null)
            {
                int assId1 = int.Parse(assId);
                a = db.Assignment.Where(b => b.assignmentId == assId1).FirstOrDefault();
                ViewBag.selectedType = a.assignmentType;
                ViewBag.selectedSecton = a.section;
                avm.submissionDate = a.submissionDate.ToShortDateString();
                avm.assignmentID = a.assignmentId;
                if (a.assignmentType == "Group")
                    avm.numberOfGroupMembers = a.numberOfMembers;
            }
            else if (update != null)
            {
                a = db.Assignment.Where(b => b.assignmentId == avm.assignmentID).FirstOrDefault();
                if (a != null)
                {

                    Section sec = new Section();
                    sec = db.Section.Where(b => b.sectionId == a.sectionID).FirstOrDefault();
                    string currentQuarter = ht.whichQuarter(sec.academicYearId);

                    if (sec.sectionId != 0 && assignmentType != null && ht.beforeQuarterEnd(Convert.ToDateTime(avm.submissionDate), sec.academicYearId) && (currentQuarter == "Q1" || currentQuarter == "Q2" || currentQuarter == "Q3" || currentQuarter == "Q4"))
                    {
                        a.sectionID = sec.sectionId;
                        a.assignmentType = assignmentType;
                        a.datePosted = DateTime.Now.Date;
                        a.submissionDate = Convert.ToDateTime(avm.submissionDate).Date;
                        if (assignmentType == "Group" && avm.numberOfGroupMembers >= 2)
                            a.numberOfMembers = avm.numberOfGroupMembers;
                        if (file != null)
                        {
                            int length = file.ContentLength;
                            byte[] upload = new byte[length];
                            file.InputStream.Read(upload, 0, length);
                            a.assignmentDocument = upload;
                        }

                        db.SaveChanges();
                        ViewBag.successfulMessage = "Assignment updated Successfully.";
                    }
                    else
                    {
                        ViewBag.failedMessage = "Update failed.";
                    }
                }
            }
            else if (delete != null)
            {

                int assId1 = int.Parse(assId);
                a = db.Assignment.Where(b => b.assignmentId == assId1).FirstOrDefault();
                db.Assignment.Remove(a);
                db.SaveChanges();
            }
            else if (btnSubmitGrouping != null)
            {
                string needed = "";
                int cnt = txtValue.Length;
                List<string> cl= new List<string>();
                needed = txtValue.Substring(1, cnt - 2);
               
                string[] theLists = needed.Split(new string[] {"-*"},StringSplitOptions.None);
                int x = 0;
                Assignment ass = new Assignment();
                ass = db.Assignment.Where(ab => ab.assignmentId == avm.assignmentID).FirstOrDefault();
                if (ass != null)
                {
                  //  ass.groupList = String.Join("##", theLists);
                    db.SaveChanges();
                }

               
            }
               
            Models.Teacher t = new Models.Teacher();
            List<Section> s = new List<Section>();
            avm.listAssignment = new List<Assignment>();
            avm.listAssignment = db.Assignment.ToList();
            avm.assignmentType = new List<String>();
            avm.assignmentType.Add("Individual");
            avm.assignmentType.Add("Group");
            avm.sectionList = new List<string>();
            t = db.Teacher.Where(b => b.teacherId == currentID).FirstOrDefault();
            string teacherGrade = t.grade.ToString();
            s = db.Section.Where(b => b.sectionName.StartsWith(teacherGrade)).ToList();
            string currentAcademicYearName = "";
            foreach (var k in s)
            {
                if (ht.isInAcademicYear(k.academicYearId))
                {
                    currentAcademicYearName = k.academicYearId;
                    break;
                }
            }
            temp = db.AcademicYear.Where(b => b.academicYearName == currentAcademicYearName).FirstOrDefault();
            string quarter = ht.whichQuarter(currentAcademicYearName);
            var currentSections = db.Section.Where(b => b.academicYearId == currentAcademicYearName);
            foreach (var k in currentSections)
            {
                avm.sectionList.Add(k.sectionName);
            }
            avm.studentList = db.Student.Where(at => at.academicYearId == currentAcademicYearName).ToList();
            int c = avm.studentList.Count();
            avm.studentArray = new string[c * 2];
            int y = 0;
            for (int x = 0; x < avm.studentList.Count(); x++)
            {

                avm.studentArray[y] = avm.studentList[x].sectionName;
                y++;
                avm.studentArray[y] = avm.studentList[x].fullName;
                y++;
            }

            return View(avm);
        }
        
        public ActionResult manageGroup()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string currentID = User.Identity.GetUserId();
            Models.Teacher t = new Models.Teacher();
            HomeroomTeacherMethod ht = new HomeroomTeacherMethod();
            AcademicYear temp = new AcademicYear();
            GroupStructure g = new GroupStructure();
            groupStructureViewModel gsvm = new groupStructureViewModel();
            gsvm.groupList = new List<GroupStructure>();
          
            gsvm.sectionList = new List<String>();
            List<Section> s = new List<Section>();
            t = db.Teacher.Where(a => a.teacherId == currentID).FirstOrDefault();
            string teacherGrade = t.grade.ToString();
            s = db.Section.Where(a => a.sectionName.StartsWith(teacherGrade)).ToList();
            string currentAcademicYearName = "";
            foreach (var k in s)
            {
                if (ht.isInAcademicYear(k.academicYearId))
                {
                    currentAcademicYearName = k.academicYearId;
                    break;
                }
            }
            temp = db.AcademicYear.Where(a => a.academicYearName == currentAcademicYearName).FirstOrDefault();
            string quarter = ht.whichQuarter(currentAcademicYearName);
            var currentSections = db.Section.Where(a => a.academicYearId == currentAcademicYearName);
           
            foreach (var k in currentSections)
            {
               
                gsvm.sectionList.Add(k.sectionName);
            }
            if (quarter == "Q1" || quarter == "Q2" || quarter == "Q3" || quarter == "Q4") { 
            string acQuarter = currentAcademicYearName + " " + quarter;
            gsvm.groupList = db.GroupStructure.Where(ax=>ax.academicQuarter ==acQuarter && ax.section.sectionName.StartsWith(teacherGrade)).ToList();
            }
            return View(gsvm);
        }
        [HttpPost]
        public ActionResult manageGroup(int x)
        {

            return View();
        }

        public ActionResult addGroupStructure()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string currentID = User.Identity.GetUserId();
            Models.Teacher t = new Models.Teacher();
            HomeroomTeacherMethod ht = new HomeroomTeacherMethod();
            AcademicYear temp = new AcademicYear();
            GroupStructure g = new GroupStructure();
            groupStructureViewModel gsvm = new groupStructureViewModel();
            gsvm.groupList = new List<GroupStructure>();
            gsvm.sectionList = new List<String>();
            List<Section> s = new List<Section>();
            t = db.Teacher.Where(a => a.teacherId == currentID).FirstOrDefault();
            string teacherGrade = t.grade.ToString();
            s = db.Section.Where(a => a.sectionName.StartsWith(teacherGrade)).ToList();
            string currentAcademicYearName = "";
            foreach (var k in s)
            {
                if (ht.isInAcademicYear(k.academicYearId))
                {
                    currentAcademicYearName = k.academicYearId;
                    break;
                }
            }
            temp = db.AcademicYear.Where(a => a.academicYearName == currentAcademicYearName).FirstOrDefault();
            string quarter = ht.whichQuarter(currentAcademicYearName);
            var currentSections = db.Section.Where(a => a.academicYearId == currentAcademicYearName);

            foreach (var k in currentSections)
            {

                gsvm.sectionList.Add(k.sectionName);
            }
            gsvm.academicQuarter = currentAcademicYearName + "-" + quarter;
            
            return View(gsvm);
        }
        [HttpPost]
        public ActionResult addGroupStructure(groupStructureViewModel gvm,string selectSections)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string currentID = User.Identity.GetUserId();
            Models.Teacher t = new Models.Teacher();
            t = db.Teacher.Where(a => a.teacherId == currentID).FirstOrDefault();
            string teacherGrade = t.grade.ToString();
            Section s = new Section();
            gvm.sectionList = new List<String>();
            List<Section> sec = new List<Section>();
            GroupStructure gs = new GroupStructure();
            gvm.groupList = new List<GroupStructure>();
            string extractedAcademicYear = gvm.academicQuarter.Substring(0, gvm.academicQuarter.IndexOf('-'));

            gvm.groupList = db.GroupStructure.Where(gr => gr.academicQuarter == gvm.academicQuarter && gr.section.sectionName.StartsWith(teacherGrade)).ToList();
            if (ModelState.IsValid) {
                
                s = db.Section.Where(a => a.sectionName == selectSections && a.academicYearId == extractedAcademicYear).FirstOrDefault();
                gs.academicQuarter = gvm.academicQuarter;
                gs.groupStructureName = gvm.groupStructureName;
                gs.maxNumberOfMembers = gvm.maxNumberOfMembers;
                gs.minNumberOfMembers = gvm.minNumberOfMembers;
                gs.sectionId = s.sectionId;
                db.GroupStructure.Add(gs);
                db.SaveChanges();
                return View("manageGroup", gvm);
            }
            sec = db.Section.Where(a => a.sectionName.StartsWith(teacherGrade)).ToList();
            var currentSections = db.Section.Where(a => a.academicYearId ==extractedAcademicYear).ToList();

            foreach (var k in currentSections)
            {

                gvm.sectionList.Add(k.sectionName);
            }
            return View(gvm);
        }
        public ActionResult classifyGroup(string grId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            groupStructureViewModel gsv = new groupStructureViewModel();
            int grId1 = int.Parse(grId);
            GroupStructure gs = new GroupStructure();
            gs = db.GroupStructure.Where(a => a.groupStructureId == grId1).FirstOrDefault();
            string[] extractingAcademicYear = new string[1];
            extractingAcademicYear = gs.academicQuarter.Split('-');
            gsv.studentList = new List<Student>();
            gsv.studentList = db.Student.Where(a => a.sectionName == gs.section.sectionName && a.academicYearId == extractingAcademicYear[0]).ToList();
            return View();
        }
    }
}
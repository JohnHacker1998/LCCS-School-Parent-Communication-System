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

        public ActionResult GradeManagement()
        {
            //basic objects
            ApplicationDbContext context = new ApplicationDbContext();
            StudentViewModel studentViewModel = new StudentViewModel();
            studentViewModel.student = new List<Student>();

            var allAcadamicYears = context.AcademicYear.ToList();
            string tId = User.Identity.GetUserId().ToString();
            var teacher = context.Teacher.Find(tId);

            if (allAcadamicYears.Count != 0)
            {
                foreach (var getAcadamicYear in allAcadamicYears)
                {
                    //check today is in between start and end date of the specific academic year
                    if (!(DateTime.Compare(DateTime.Now, getAcadamicYear.durationStart) < 0 || DateTime.Compare(DateTime.Now, getAcadamicYear.durationEnd) > 0))
                    {
                        var listOfSections = context.Section.Where(s => s.academicYearId == getAcadamicYear.academicYearName && s.sectionName.StartsWith(teacher.grade.ToString())).ToList();
                        if (listOfSections.Count > 0)
                        {
                            foreach (var getSection in listOfSections)
                            {
                                var studentinSection = context.Student.Where(s=>s.sectionName==getSection.sectionName && s.academicYearId==getAcadamicYear.academicYearName).ToList();

                                if (studentinSection.Count != 0)
                                {
                                    foreach (var getStudents in studentinSection)
                                    {
                                        studentViewModel.student.Add(getStudents);
                                    }
                                }                                
          
                            }
                        }
                    }
                }
            }

            //give zero to assesements on the gradeManagement------------------------------------

            return View(studentViewModel);
        }

        public ActionResult AddGrade(string id)
        {

            //need to get assignment submittion dates past same quarter
            //need past exam schedules same quarter
            //check exam schedule attendance student present or absent
            //date-which one format

            //retrive the dates


            //dates,result,feedback

            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            HomeroomTeacherMethod homeroomTeacherMethod = new HomeroomTeacherMethod();
            AddGradeModal addGradeModal = new AddGradeModal();
            addGradeModal.dates = new List<string>();

            string tId = User.Identity.GetUserId().ToString();
            var teacher = context.Teacher.Find(tId);
            addGradeModal.studentId = int.Parse(id);

            Section identifyYear = new Section();

            var allAcadamicYears = context.AcademicYear.ToList();

            foreach (var getAcadamicYear in allAcadamicYears)
            {
                //check today is in between start and end date of the specific academic year
                if (!(DateTime.Compare(DateTime.Now, getAcadamicYear.durationStart) < 0 || DateTime.Compare(DateTime.Now, getAcadamicYear.durationEnd) > 0))
                {
                    identifyYear = context.Section.Where(s => s.sectionName.StartsWith(teacher.grade.ToString()) && s.academicYearId == getAcadamicYear.academicYearName).FirstOrDefault();
                    if (identifyYear != null)
                    {
                        break;
                    }
                }
            }

            var getYear = context.AcademicYear.Find(identifyYear.academicYearId);
            var quarter = homeroomTeacherMethod.whichQuarter(identifyYear.academicYearId);
            
            var schedules = context.Schedule.Where(s=>s.academicYear==getYear.academicYearName+"-"+quarter && (s.subject==teacher.subject || s.subject=="All") && s.grade==teacher.grade).ToList();

            if (schedules.Count != 0)
            {
                foreach (var getSchedule in schedules)
                {
                    if (DateTime.Compare(DateTime.Now.Date, getSchedule.scheduleDate.Date) > 0)
                    {
                        addGradeModal.dates.Add(getSchedule.scheduleDate.ToShortDateString()+"-"+getSchedule.scheduleFor);

                        //if(getSchedule.scheduleFor!= "Reassessment")
                        //{
                        //    var absentStudent = context.AbsenceRecord.Where(a=>a.recordDate==getSchedule.scheduleDate.Date && a.student.sectionName.StartsWith(teacher.grade.ToString()) && a.academicPeriod==getYear.academicYearName+"-"+quarter && a.evidenceFlag==null).ToList();

                        //    if (absentStudent.Count!=0)
                        //    {
                        //        foreach(var getStudent in absentStudent)
                        //        {
                        //            DateTime nextDay = getSchedule.scheduleDate.Add(TimeSpan.FromDays(1)).Date;

                        //            var absent = context.AbsenceRecord.Where(a => a.studentId == getStudent.studentId && a.recordDate == nextDay && a.evidenceFlag == null).FirstOrDefault();
                        //        }
                        //    }
                        //}
                    }
                    
                }
            }

            var assignments = context.Assignment.Where(a => a.yearlyQuarter == getYear.academicYearName + "-" + quarter && a.teacher.subject == teacher.subject && a.teacher.grade == teacher.grade).ToList();

            if (assignments.Count != 0)
            {
                foreach (var getAssignments in assignments)
                {
                    if (DateTime.Compare(DateTime.Now.Date, getAssignments.submissionDate.Date) > 0)
                    {
                        addGradeModal.dates.Add(getAssignments.submissionDate.ToShortDateString()+"-"+getAssignments.assignmentType);
                    }

                }
            }


            return PartialView("AddGrade",addGradeModal);
        }

        [HttpPost]
        public ActionResult AddGrade(AddGradeModal addGradeModal,string dates)
        {

            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            Section identifyYear = new Section();
            HomeroomTeacherMethod homeroomTeacherMethod = new HomeroomTeacherMethod();
            Result result = new Result();

            //continious assesement and final exam check attendance percentage
            //reassesement check attendance percentage if two add them up
            //assignment check attendance percentage
            //check duplicate
            //give zero to assesements on the gradeManagement

            string tId = User.Identity.GetUserId().ToString();
            var teacher = context.Teacher.Find(tId);
            string[] dateTaken = dates.Split('-');
            DateTime date = DateTime.Parse(dateTaken[0]).Date;

            //Reassessment

            if (dateTaken[1]== "Continious Assessment Test" || dateTaken[1]== "Final Exam")
            {
                var schedule = context.Schedule.Where(s=>s.scheduleDate==date && s.grade==teacher.grade && s.subject==teacher.subject).FirstOrDefault();
                var attendance = context.AbsenceRecord.Where(a => a.studentId == addGradeModal.studentId && a.recordDate == date).FirstOrDefault();

                if (attendance == null)
                {
                    if (addGradeModal.result <= schedule.percentage)
                    {
                        var duplicate = context.Result.Where(r => r.scheduleId == schedule.scheduleId && r.studentId == addGradeModal.studentId).FirstOrDefault();

                        if (duplicate == null)
                        {
                            result.teacherId = tId;
                            result.studentId = addGradeModal.studentId;
                            result.result = addGradeModal.result;
                            result.feedback = addGradeModal.feedback;
                            result.scheduleId = schedule.scheduleId;
                            result.assignmentId = null;
                            result.resultFor = dateTaken[1];
                            result.percent = schedule.percentage;
                            result.academicYear = schedule.academicYear;
                            result.grade = teacher.grade;

                            context.Result.Add(result);
                            int success= context.SaveChanges();

                            if (success > 0)
                            {
                                ViewBag.complete = "Result Recorded Successfully";
                            }
                            else
                            {
                                //error failed to record result
                                ViewBag.error = "Failed to Record Result!!";
                            }
                        }
                        else
                        {
                            //error result already exist
                            ViewBag.error = "Result Record Already Exist!!";
                        }
                    }
                    else
                    {
                        //error result excced percentage
                        ViewBag.error = "Result Exceed the Score limit";
                    }
                }
                else
                {
                    //error absent on that day
                    ViewBag.error = "Student Didn't Take the Assessment";
                }
            }
            else if (dateTaken[1]=="Group" || dateTaken[1]=="Individual")
            {
                var assignment = context.Assignment.Where(a=>a.submissionDate==date && a.teacher.grade==teacher.grade && a.teacher.subject==teacher.subject).FirstOrDefault();
                var attendance = context.AbsenceRecord.Where(a => a.studentId == addGradeModal.studentId && a.recordDate == date && a.evidenceFlag==null).FirstOrDefault();

                if (attendance == null)
                {
                    if (addGradeModal.result <= assignment.markPercentage)
                    {
                        var duplicate = context.Result.Where(r => r.assignmentId == assignment.assignmentId && r.studentId == addGradeModal.studentId).FirstOrDefault();

                        if (duplicate == null)
                        {
                            result.teacherId = tId;
                            result.studentId = addGradeModal.studentId;
                            result.result = addGradeModal.result;
                            result.feedback = addGradeModal.feedback;
                            result.scheduleId = null;
                            result.assignmentId = assignment.assignmentId;
                            result.resultFor = dateTaken[1];
                            result.percent = assignment.markPercentage;
                            result.academicYear = assignment.yearlyQuarter;
                            result.grade = teacher.grade;

                            context.Result.Add(result);
                            int success = context.SaveChanges();

                            if (success > 0)
                            {
                                ViewBag.complete = "Result Recorded Successfully";
                            }
                            else
                            {
                                //error failed to record result
                                ViewBag.error = "Failed to Record Result!!";
                            }
                        }
                        else
                        {
                            //error result already exist
                            ViewBag.error = "Result Record Already Exist!!";
                        }
                    }
                    else
                    {
                        //error result excced percentage
                        ViewBag.error = "Result Exceed the Score limit";
                    }
                }
                else
                {
                    //error absent on that day
                    ViewBag.error = "Student Didn't Take the Assessment";
                }
            }
            else if (dateTaken[1]== "Reassessment")
            {
                //get all miss by subject
                var allAcadamicYears = context.AcademicYear.ToList();

                foreach (var getAcadamicYear in allAcadamicYears)
                {
                    //check today is in between start and end date of the specific academic year
                    if (!(DateTime.Compare(DateTime.Now, getAcadamicYear.durationStart) < 0 || DateTime.Compare(DateTime.Now, getAcadamicYear.durationEnd) > 0))
                    {
                        identifyYear = context.Section.Where(s => s.sectionName.StartsWith(teacher.grade.ToString()) && s.academicYearId == getAcadamicYear.academicYearName).FirstOrDefault();
                        if (identifyYear != null)
                        {
                            break;
                        }
                    }
                }

                var getYear = context.AcademicYear.Find(identifyYear.academicYearId);
                var quarter = homeroomTeacherMethod.whichQuarter(identifyYear.academicYearId);
                int sum = 0;

                var reassessment = context.Schedule.Where(s => s.scheduleDate == date && s.scheduleFor == "Reassessment").FirstOrDefault();
                var schedule = context.Schedule.Where(s=>s.academicYear==getYear.academicYearName+"-"+quarter && s.grade==teacher.grade && s.subject==teacher.subject).ToList();
                if (schedule.Count != 0)
                {
                    foreach (var getSchedules in schedule)
                    {
                        var getPercent = context.AbsenceRecord.Where(a => a.studentId == addGradeModal.studentId && a.academicPeriod == getYear.academicYearName + "-" + quarter && a.recordDate == getSchedules.scheduleDate && a.evidenceFlag== "AcceptableReason").FirstOrDefault();

                        if (getPercent!=null)
                        {
                            sum += getSchedules.percentage;
                        }
                    }

                    if (sum != 0)
                    {
                        var duplicate = context.Result.Where(r => r.scheduleId == reassessment.scheduleId && r.studentId == addGradeModal.studentId).FirstOrDefault();

                        if (duplicate == null)
                        {
                            result.teacherId = tId;
                            result.studentId = addGradeModal.studentId;
                            result.result = addGradeModal.result;
                            result.feedback = addGradeModal.feedback;
                            result.scheduleId = reassessment.scheduleId;
                            result.assignmentId = null;
                            result.resultFor = dateTaken[1];
                            result.percent = sum;
                            result.academicYear = reassessment.academicYear;
                            result.grade = teacher.grade;

                            context.Result.Add(result);
                            int success = context.SaveChanges();

                            if (success > 0)
                            {
                                ViewBag.complete = "Result Recorded Successfully";
                            }
                            else
                            {
                                //error failed to record result
                                ViewBag.error = "Failed to Record Result!!";
                            }
                        }
                        else
                        {
                            //error result already exist
                            ViewBag.error = "Result Record Already Exist!!";
                        }
                    }
                    else
                    {
                        //error student not valid for reassesement
                        ViewBag.error = "Student not Valid for Reassessment";
                    }
                }
                else
                {
                    //error you cant give reassesement result(no schedules before reassesement)
                    ViewBag.error = "You Don't have Any Previous Assesement to Give Reassessment";
                }

            }

            return PartialView("AddGrade", addGradeModal);
        }

        public ActionResult UpdateResultManagement()
        {
            //get students previous current quarter same teacher

            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            UpdateResultViewModel updateResultViewModel = new UpdateResultViewModel();
            updateResultViewModel.results = new List<Result>();
            HomeroomTeacherMethod homeroomTeacherMethod = new HomeroomTeacherMethod();

            string tId = User.Identity.GetUserId().ToString();
            var teacher = context.Teacher.Find(tId);

            Section identifyYear = new Section();

            var allAcadamicYears = context.AcademicYear.ToList();

            foreach (var getAcadamicYear in allAcadamicYears)
            {
                //check today is in between start and end date of the specific academic year
                if (!(DateTime.Compare(DateTime.Now, getAcadamicYear.durationStart) < 0 || DateTime.Compare(DateTime.Now, getAcadamicYear.durationEnd) > 0))
                {
                    identifyYear = context.Section.Where(s => s.sectionName.StartsWith(teacher.grade.ToString()) && s.academicYearId == getAcadamicYear.academicYearName).FirstOrDefault();
                    if (identifyYear != null)
                    {
                        break;
                    }
                }
            }

            var getYear = context.AcademicYear.Find(identifyYear.academicYearId);
            var quarter = homeroomTeacherMethod.whichQuarter(identifyYear.academicYearId);

            var results = context.Result.Where(r=>r.teacherId==tId && r.academicYear==getYear.academicYearName+"-"+quarter).ToList();
            if (results.Count != 0)
            {
                foreach(var getResults in results)
                {
                    updateResultViewModel.results.Add(getResults);
                }
            }

            return View(updateResultViewModel);
        }

        public ActionResult UpdateResult(string id)
        {

            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            UpdateGradeModal updateGradeModal = new UpdateGradeModal();

            int rId = int.Parse(id);

            var result = context.Result.Find(rId);

            updateGradeModal.result = result.result;
            updateGradeModal.feedback = result.feedback;
            updateGradeModal.resultId = rId;

            return PartialView("UpdateResult",updateGradeModal);
        }

        [HttpPost]
        public ActionResult UpdateResult(UpdateGradeModal updateGradeModal)
        {
            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();

            var result = context.Result.Find(updateGradeModal.resultId);

            if (updateGradeModal.result<=result.percent)
            {
                result.result = updateGradeModal.result;
                result.feedback = updateGradeModal.feedback;

                int success= context.SaveChanges();
                if (success > 0)
                {
                    ViewBag.complete = "Result Updated Successfully";
                }
                else
                {
                    ViewBag.error = "Failed to Update Result";
                }
            }

            return PartialView("UpdateResult",updateGradeModal);

        }
    }
}
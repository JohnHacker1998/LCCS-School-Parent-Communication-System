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
            ViewBag.cannothaveSections = " ";

            List<Section> s = new List<Section>();
            avm.listAssignment = new List<Assignment>();
            avm.listAssignment = db.Assignment.ToList();
           
           
            
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
           if (quarter == "Q1" || quarter == "Q2" || quarter == "Q3" || quarter == "Q4") {
                var currentSections = db.Section.Where(a => a.academicYearId == currentAcademicYearName).ToList();
                foreach (var k in currentSections)
                {
                    avm.sectionList.Add(k.sectionName);
                }
                avm.studentList = db.Student.Where(a => a.academicYearId == currentAcademicYearName).ToList();
                string acQuarter = currentAcademicYearName + "-" + quarter;

                
            }
            else
            {
                ViewBag.cannothaveSections="This date doesn't exist in the current Academic Year";
            }
            

            return View(avm);
        }
       
        public bool fulfillsPercentage(string yearlyQuarter, int sectionID, string teacherID, int percentage)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List <Assignment> listAssignment= new List<Assignment>();
            listAssignment = db.Assignment.Where(ax => ax.yearlyQuarter == yearlyQuarter && ax.sectionID == sectionID && ax.teacherId == teacherID).ToList();
            int sum = 0;
            if (listAssignment != null) { 
            foreach(var k in listAssignment)
            {
                    sum += k.markPercentage;
            }
            }
            if (sum + percentage <= 100)
            {
                return true;
            }
            return false;
        }
      
        [HttpPost]
        public ActionResult addAssignment(assignmentViewModel avm, HttpPostedFileBase file, string assignmentType, string sectionName,string fileName)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            HomeroomTeacherMethod ht = new HomeroomTeacherMethod();
            Assignment a = new Assignment();
            Models.Teacher t = new Models.Teacher();

            AcademicYear temp = new AcademicYear();
            avm.studentList = new List<Student>();
            avm.listAssignment = new List<Assignment>();
            avm.gsList = new List<GroupStructure>();


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

            if (currentSectionID != 0 && ht.beforeQuarterEnd(Convert.ToDateTime(avm.submissionDate), theSection.academicYearId) && (currentQuarter == "Q1" || currentQuarter == "Q2" || currentQuarter == "Q3" || currentQuarter == "Q4") && fulfillsPercentage(theSection.academicYearId + "-" + currentQuarter, currentSectionID, currentTeacherID, avm.markPercentage))
            {
                a.sectionID = currentSectionID;
                a.assignmentType = "Individual";
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
                a.fileName = fileName;
                db.Assignment.Add(a);
                db.SaveChanges();
                ViewBag.successfulMessage = "Assignment added Successfully.";
                avm.listAssignment = db.Assignment.Where(x => x.teacherId == currentTeacherID && x.yearlyQuarter == a.yearlyQuarter).ToList();
                avm.gsList = db.GroupStructure.Where(x => x.teacherId == currentTeacherID && x.academicQuarter == a.yearlyQuarter && x.completeStatus == 1).ToList();
                return View("assignmentManagement", avm);
            }
            else
            {
                if (currentQuarter == "no")
                {
                    ViewBag.failedMessage = "Quarter doesn't exist";

                }
                if (currentQuarter == "gap")
                {
                    ViewBag.failedMessage = "Current timespan is a gap";
                }
                if (fulfillsPercentage(theSection.academicYearId + "-" + currentQuarter, currentSectionID, currentTeacherID, avm.markPercentage) == false)
                {
                    ViewBag.doesntFulfillMessage = "You have already reached the maximum assignment Mark Percentage.";
                }

            }
          
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
        [HttpGet]
        public ActionResult addGroupAssignment(string gsId,string txtAdd)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            assignmentViewModel avm = new assignmentViewModel();
            avm.groupList = new List<Group>();
            int gsId1 = int.Parse(txtAdd);
            GroupStructure gs = new GroupStructure();
            gs = db.GroupStructure.Where(ax=>ax.groupStructureId==gsId1).FirstOrDefault();
           
            avm.groupList= db.Group.Where(ax=>ax.groupStrId==gsId1).ToList();
            avm.groupStructureId = gs.groupStructureId;

            return View(avm);
        }
        [HttpPost]
        public ActionResult addGroupAssignment(string selectedStudents,assignmentViewModel avm, string add, HttpPostedFileBase file,string fileName)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            GroupStructure gs = new GroupStructure();
            HomeroomTeacherMethod ht = new HomeroomTeacherMethod();
    
            GroupStructureAssignment gsa = new GroupStructureAssignment();
            Assignment ass = new Assignment();
            List<Group> gList = new List<Group>();
            //GroupStructure gs
            string teacherID = User.Identity.GetUserId();
            Models.Teacher t = new Models.Teacher();
            t = db.Teacher.Where(ax => ax.teacherId == teacherID).FirstOrDefault();
           
            gs = db.GroupStructure.Where(ax => ax.groupStructureId == avm.groupStructureId).FirstOrDefault();
            string[] arr = new string[1];
            arr = gs.academicQuarter.Split('-');
            string aYear = arr[0];
            if (add != null)
            {
                if (fulfillsPercentage(gs.academicQuarter, gs.sectionId, teacherID, avm.markPercentage) == true && ht.beforeQuarterEnd(Convert.ToDateTime(avm.submissionDate), aYear))
                {
                    ass.datePosted = DateTime.Now.Date;
                    ass.yearlyQuarter = gs.academicQuarter;
                    ass.assignmentType = "Group";
                    ass.sectionID = gs.sectionId;
                    ass.submissionDate = Convert.ToDateTime(avm.submissionDate).Date;
                    ass.assignmentName = avm.assignmentName;
                    ass.teacherId = User.Identity.GetUserId();
                    int length = file.ContentLength;
                    byte[] upload = new byte[length];
                    file.InputStream.Read(upload, 0, length);
                    ass.assignmentDocument = upload;
                    ass.fileName = fileName;
                    ass.markPercentage = avm.markPercentage;
                    
                    db.Assignment.Add(ass);
                    db.SaveChanges();
                    String firstSeries = selectedStudents.Substring(1, selectedStudents.Length - 2);

                    var theAssignment = db.Assignment.Where(ax => ax.teacherId == teacherID && ax.yearlyQuarter == gs.academicQuarter && ax.sectionID == gs.sectionId && ax.datePosted == ass.datePosted && ax.submissionDate == ass.submissionDate).FirstOrDefault();
                    List<string> memList = new List<string>();
                    memList = firstSeries.Split('-').ToList();

                    gList = db.Group.Where(ax => ax.groupStrId == gs.groupStructureId).ToList();
                    if (memList.Count == gList.Count)
                    {
                        gsa.assignmentId = theAssignment.assignmentId;
                        gsa.groupStructureId = gs.groupStructureId;
                        db.GroupStructureAssignment.Add(gsa);
                        db.SaveChanges();
                    }
                    else
                    {
                       
                        foreach(var n in memList)
                        {
                            GroupAssignment ga = new GroupAssignment();
                            Group gri = new Group();
                            gri = db.Group.Where(ax => ax.groupName == n).FirstOrDefault();
                            ga.assignmentId = theAssignment.assignmentId;
                            ga.grId = gri.groupId;
                            db.GroupAssignment.Add(ga);
                            db.SaveChanges();

                        }
                        
                       

                    }
                    string teacherGrade = t.grade.ToString();
                    avm.gsList = new List<GroupStructure>();
                    avm.gsList = db.GroupStructure.Where(ax => ax.teacherId == teacherID && ax.academicQuarter == gs.academicQuarter && ax.completeStatus == 1 && ax.section.sectionName.StartsWith(teacherGrade)).ToList();
                    avm.listAssignment = db.Assignment.Where(ax => ax.teacherId == teacherID && ax.yearlyQuarter == gs.academicQuarter).ToList();
                    return View("assignmentManagement", avm);
                }
            }
            avm.groupList = db.Group.Where(ax => ax.groupStrId == gs.groupStructureId).ToList();
            avm.groupStructureId = gs.groupStructureId;

            return View(avm);
        }
        [HttpGet]
        public ActionResult updateAssignment(string assId, string txtAssignmentID)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            int assId1 = int.Parse(txtAssignmentID);
            HomeroomTeacherMethod ht = new HomeroomTeacherMethod();
            Assignment ass = new Assignment();
            ass = db.Assignment.Where(ax => ax.assignmentId == assId1).FirstOrDefault();
            assignmentViewModel avm = new assignmentViewModel();
            avm.assignmentID = ass.assignmentId;
            avm.assignmentName = ass.assignmentName;
            avm.markPercentage = ass.markPercentage;
            avm.submissionDate = ass.submissionDate.ToShortDateString();
            
            return View(avm);
        }
        [HttpPost]
        public ActionResult updateAssignment(string fileName, assignmentViewModel avm, HttpPostedFileBase file)
        {
            string teacherID = User.Identity.GetUserId();
            ApplicationDbContext db = new ApplicationDbContext();
            HomeroomTeacherMethod ht = new HomeroomTeacherMethod();
            Assignment ass = new Assignment();
            avm.listAssignment = new List<Assignment>();
            Models.Teacher t = new Models.Teacher();
            t = db.Teacher.Where(ax => ax.teacherId == teacherID).FirstOrDefault();
            string teacherGrade = t.grade.ToString();
            avm.gsList = new List<GroupStructure>();
            ass = db.Assignment.Where(ax => ax.assignmentId == avm.assignmentID).FirstOrDefault();
            string[] arr = new string[1];
            arr = ass.yearlyQuarter.Split('-');
            string aYear = arr[0];
            if (fulfillsPercentage(ass.yearlyQuarter, ass.sectionID, ass.teacherId, avm.markPercentage) == true && ht.beforeQuarterEnd(Convert.ToDateTime(avm.submissionDate), aYear))
            {
                ass.assignmentName = avm.assignmentName;
                ass.submissionDate = Convert.ToDateTime(avm.submissionDate).Date;
                ass.markPercentage = avm.markPercentage;
                if (fileName != null)
                {
                    int length = file.ContentLength;
                    byte[] upload = new byte[length];
                    file.InputStream.Read(upload, 0, length);
                    ass.assignmentDocument = upload;
                    ass.fileName = fileName;
                }
                db.SaveChanges();
                avm.gsList = db.GroupStructure.Where(ax => ax.teacherId == teacherID && ax.academicQuarter == ass.yearlyQuarter && ax.completeStatus == 1 && ax.section.sectionName.StartsWith(teacherGrade)).ToList();
                avm.listAssignment = db.Assignment.Where(ax => ax.teacherId == teacherID && ax.yearlyQuarter == ass.yearlyQuarter).ToList();
                return View("assignmentManagement", avm);

            }

            return View(avm);
        }
        public ActionResult assignmentManagement()
        {
            /*ViewBag.selectedType = " ";
            ViewBag.selectedSection = " ";
          
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
            int c = avm.studentList.Count();
            avm.studentArray = new string[c * 2];
            int st = 0;
            *//*   foreach(var k in avm.studentList)
               {
                  if(st<avm.studentArray.Length)
                   { 
                   avm.studentArray[st] = k.sectionName;
                   avm.studentArray[st + 1] = k.fullName;

                   }
                   st++;
               }*//*
            int y = 0;
            for (int x = 0; x < avm.studentList.Count(); x++)
            {

                avm.studentArray[y] = avm.studentList[x].sectionName;
                y++;
                avm.studentArray[y] = avm.studentList[x].fullName;
                y++;
            }*/
            HomeroomTeacherMethod ht = new HomeroomTeacherMethod();
            ApplicationDbContext db = new ApplicationDbContext();           
            assignmentViewModel avm = new assignmentViewModel();
            avm.gsList = new List<GroupStructure>();
            string currentID = User.Identity.GetUserId();
            Models.Teacher t = new Models.Teacher();
            t = db.Teacher.Where(ax => ax.teacherId == currentID).FirstOrDefault();
            string teacherGrade = t.grade.ToString();
            AcademicYear temp = new AcademicYear();
            List<Section> s = new List<Section>();
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
            string academicQuarter = currentAcademicYearName + "-" + quarter;
            if (quarter == "Q1" || quarter == "Q2" || quarter == "Q3" || quarter == "Q4")
            {

            }
            avm.gsList = db.GroupStructure.Where(gr=>gr.academicQuarter==academicQuarter && gr.section.sectionName.StartsWith(teacherGrade) && gr.completeStatus==1).ToList();
            avm.listAssignment = new List<Assignment>();
            avm.listAssignment = db.Assignment.Where(ax=>ax.yearlyQuarter==academicQuarter && ax.teacherId==currentID).ToList();


                return View(avm);
        }

        [HttpPost]
        public ActionResult assignmentManagement(assignmentViewModel avm, string add, HttpPostedFileBase file, string assignmentType, string sectionName, string select, string delete, string assId, string update, string btnSubmitGrouping, string txtValue)
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
            string teacherID = User.Identity.GetUserId();
            Models.Teacher t = new Models.Teacher();
            t = db.Teacher.Where(ax => ax.teacherId == currentID).FirstOrDefault();
            string teacherGrade = t.grade.ToString();

            int assId1 = int.Parse(assId);
            a = db.Assignment.Where(b => b.assignmentId == assId1).FirstOrDefault();

            if (delete != null)
            {

                
                if (a.assignmentType == "Individual")
                {
                    db.Assignment.Remove(a);
                    db.SaveChanges();
                }
                else
                {
                    List<GroupAssignment> gaList = new List<GroupAssignment>();
                    gaList = db.GroupAssignment.Where(ax => ax.assignmentId == assId1).ToList();
                    List<GroupStructureAssignment> gsaList = new List<GroupStructureAssignment>();
                    gsaList = db.GroupStructureAssignment.Where(ax => ax.assignmentId == assId1).ToList();
                    if (gaList != null)
                    {
                        foreach(var k in gaList)
                        {
                            db.GroupAssignment.Remove(k);
                            db.SaveChanges();
                        }
                    }
                    if (gsaList != null)
                    {
                        foreach(var k in gsaList)
                        {
                            db.GroupStructureAssignment.Remove(k);
                            db.SaveChanges();
                        }
                    }
                    db.Assignment.Remove(a);
                    db.SaveChanges();

                }
                
            }

            avm.gsList = new List<GroupStructure>();
            avm.listAssignment = new List<Assignment>();
            avm.gsList = db.GroupStructure.Where(ax => ax.teacherId == teacherID && ax.academicQuarter == a.yearlyQuarter && ax.completeStatus == 1 && ax.section.sectionName.StartsWith(teacherGrade)).ToList();
            avm.listAssignment = db.Assignment.Where(ax => ax.teacherId == teacherID && ax.yearlyQuarter == a.yearlyQuarter).ToList();

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
                string acQuarter = currentAcademicYearName + "-" + quarter;
                gsvm.groupList = db.GroupStructure.Where(ax => ax.academicQuarter == acQuarter && ax.section.sectionName.StartsWith(teacherGrade)).ToList();
            }
            return View(gsvm);
        }
        [HttpPost]
        public ActionResult manageGroup(string grId, string delete)
        {
            string teacherId = User.Identity.GetUserId();
            ViewBag.cannotDelete = " ";
            ApplicationDbContext db = new ApplicationDbContext();
            HomeroomTeacherMethod ht = new HomeroomTeacherMethod();
            int idX1 = int.Parse(grId);
            GroupStructure gs = new GroupStructure();
            Models.Teacher t = new Models.Teacher();
            t = db.Teacher.Where(ax=>ax.teacherId==teacherId).FirstOrDefault();
            string teacherGrade = t.grade.ToString();
            groupStructureViewModel gsvm = new groupStructureViewModel();
            gsvm.groupList = new List<GroupStructure>();

            gs = db.GroupStructure.Where(ax => ax.groupStructureId == idX1).FirstOrDefault();
            
            if (delete != null)
            {               
             GroupStructureAssignment gsa = new GroupStructureAssignment();
                gsa = db.GroupStructureAssignment.Where(ax=>ax.groupStructureId==gs.groupStructureId).FirstOrDefault();
                if (gsa == null)
                {
                    db.GroupStructure.Remove(gs);
                    db.SaveChanges();

                }
                else
                {
                    ViewBag.cannotDelete = "Cannot Delete the record";
                }
                gsvm.groupList = db.GroupStructure.Where(ax => ax.academicQuarter == gs.academicQuarter && ax.section.sectionName.StartsWith(teacherGrade) && ax.teacherId==teacherId).ToList();

            }
            return View(gsvm);
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
        public ActionResult addGroupStructure(groupStructureViewModel gvm, string selectSections)
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

           
            if (ModelState.IsValid) {

                s = db.Section.Where(a => a.sectionName == selectSections && a.academicYearId == extractedAcademicYear).FirstOrDefault();
                gs.academicQuarter = gvm.academicQuarter;
                gs.groupStructureName = gvm.groupStructureName;
                gs.maxNumberOfMembers = gvm.maxNumberOfMembers;
                gs.minNumberOfMembers = gvm.minNumberOfMembers;
                gs.sectionId = s.sectionId;
                gs.teacherId = currentID;
                db.GroupStructure.Add(gs);
                db.SaveChanges();
                gvm.groupList = db.GroupStructure.Where(gr => gr.academicQuarter == gvm.academicQuarter && gr.section.sectionName.StartsWith(teacherGrade) && gr.teacherId==currentID).ToList();
                return View("manageGroup", gvm);
            }
            sec = db.Section.Where(a => a.sectionName.StartsWith(teacherGrade)).ToList();
            var currentSections = db.Section.Where(a => a.academicYearId == extractedAcademicYear).ToList();

            foreach (var k in currentSections)
            {

                gvm.sectionList.Add(k.sectionName);
            }
            return View(gvm);
        }
        [HttpGet]
        public ActionResult classifyGroup(string grIdOne,string txtImportant)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            groupViewModel gvm = new groupViewModel();
           int grId1 = int.Parse(txtImportant);
            GroupStructure gs = new GroupStructure();
            gvm.groupMembers = new List<StudentGroupList>();
            ViewBag.enableMultiGroup = false;
            gvm.groupList = new List<Group>();
            gs = db.GroupStructure.Where(a => a.groupStructureId == grId1).FirstOrDefault();
            string[] extractingAcademicYear = new string[1];
            extractingAcademicYear = gs.academicQuarter.Split('-');
            gvm.studentList = new List<Student>();
            string acadYear = extractingAcademicYear[0];
           var stdList = db.Student.Where(a => a.sectionName == gs.section.sectionName && a.academicYearId == acadYear).ToList();
            foreach(var ks in stdList)
            {
                var tempGroupMember = db.StudentGroupList.Where(a => a.studentId == ks.studentId).FirstOrDefault();
                if (tempGroupMember == null)
                {
                    
                    gvm.studentList.Add(ks);
                }
            }
            if (gvm.studentList.Count() == 0)
            {
                ViewBag.enableMultiGroup = true;
            }
            gvm.minMembers = gs.minNumberOfMembers;
            gvm.maxMembers = gs.maxNumberOfMembers;
            gvm.groupList = db.Group.Where(a => a.groupStrId == gs.groupStructureId).ToList();
            gvm.groupMembers = db.StudentGroupList.ToList();
            gvm.groupStructureID = gs.groupStructureId;
            return View(gvm);
        }
        [HttpPost]
        public ActionResult classifyGroup(string grID,string skr,groupViewModel gvm, string txtValue,string add,string delete, string btnSubmitGrouping)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Models.Teacher t = new Models.Teacher();
            ViewBag.ErrorMembers = " ";
            ViewBag.successMessage = " ";
            ViewBag.ErrorExists = " ";
            ViewBag.enableMultiGroup = false;
            Group addableGroup = new Group();
            gvm.groupList = new List<Group>();
            gvm.groupMembers = new List<StudentGroupList>();
            gvm.studentList = new List<Student>();
            Student std = new Student();
            string teacherID = User.Identity.GetUserId();
            t = db.Teacher.Where(ax => ax.teacherId == teacherID).FirstOrDefault();
            Group g = new Group();
           
            GroupStructure gs = new GroupStructure();
            gs = db.GroupStructure.Where(a => a.groupStructureId == gvm.groupStructureID).FirstOrDefault();
            if (add != null)
            {

                var tempG = db.Group.Where(a => a.groupName == gvm.groupName && a.groupStrId == gs.groupStructureId).FirstOrDefault();
                if (tempG == null && txtValue!="*") {
                    
                    String firstSeries = txtValue.Substring(1, txtValue.Length - 2);

                    
                    List<string> memList = new List<string>();
                    memList = firstSeries.Split('-').ToList();
                    if (memList.Count <= gvm.maxMembers && memList.Count >= gvm.minMembers)
                    {
                        addableGroup.groupName = gvm.groupName;
                addableGroup.groupStrId = gvm.groupStructureID;
                db.Group.Add(addableGroup);
                db.SaveChanges();

              
               
                    foreach (var k in memList)
                    {
                        StudentGroupList sgl = new StudentGroupList();
                        std = db.Student.Where(a => a.fullName == k).FirstOrDefault();
                        var tempGroup = db.Group.Where(a => a.groupName == addableGroup.groupName && a.groupStrId == addableGroup.groupStrId).FirstOrDefault();
                        sgl.groupId = tempGroup.groupId;
                        sgl.studentId = std.studentId;
                        db.StudentGroupList.Add(sgl);
                        db.SaveChanges();
                        ViewBag.successMessage = "Group Added Successfully";
                    }
                }

                else
                {
                    ViewBag.ErrorMembers = "The number of members isn't valid";
                }
            }
                else
                {
                    ViewBag.ErrorExists = "Group Name Exists.";
                }
            }
            else if (delete != null)
            {
                int grID1 = int.Parse(grID);
                g = db.Group.Where(gr => gr.groupId == grID1).FirstOrDefault();
                var grList = db.StudentGroupList.Where(a => a.groupId == g.groupId).ToList();
                foreach(var k in grList)
                {
                    db.StudentGroupList.Remove(k);
                    db.SaveChanges(); 
                }
                db.Group.Remove(g);
                db.SaveChanges();
                ViewBag.successMessage = "Group Deleted Successfully";
            }
            else if (btnSubmitGrouping != null)
            {
                gs.completeStatus = 1;
                db.SaveChanges();
                string teacherGrade = t.grade.ToString();
                groupStructureViewModel gsvm = new groupStructureViewModel();
                gsvm.groupList= new List<GroupStructure>();
                gsvm.sectionList = new List<string>();
                gsvm.studentList = new List<Student>();
                gsvm.groupList = db.GroupStructure.Where(ax => ax.academicQuarter == gs.academicQuarter && ax.section.sectionName.StartsWith(teacherGrade)).ToList();
                return View("manageGroup", gsvm);
            }
            string[] extractingAcademicYear = new string[1];
            extractingAcademicYear = gs.academicQuarter.Split('-');
            string acadYear = extractingAcademicYear[0];

            Section s = new Section();
            s = db.Section.Where(xr => xr.sectionId == gs.sectionId).FirstOrDefault();
            var tempStudentList = db.Student.Where(a => a.sectionName == s.sectionName && a.academicYearId== acadYear).ToList();
            foreach(var k in tempStudentList)
            {
                var tempGroupMember = db.StudentGroupList.Where(a => a.studentId == k.studentId).FirstOrDefault();
                if (tempGroupMember == null)
                {
                    
                    gvm.studentList.Add(k);
                }
            }
            gvm.groupMembers = db.StudentGroupList.Where(a => a.group.groupStrId == gvm.groupStructureID).ToList();
            gvm.groupList = db.Group.Where(a => a.groupStrId == gvm.groupStructureID).ToList();
            if (gvm.studentList.Count() == 0)
            {
                ViewBag.enableMultiGroup = true;
            }
            gvm.maxMembers = gs.maxNumberOfMembers;
            gvm.minMembers = gs.minNumberOfMembers;
            gvm.groupStructureID = gs.groupStructureId;
            return View(gvm);
        }

        [HttpGet]
        public ActionResult viewGroup(string grIdOne, string txtImportant)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            groupViewModel gvm = new groupViewModel();
            int grId1 = int.Parse(txtImportant);
            GroupStructure gs = new GroupStructure();
            gvm.groupMembers = new List<StudentGroupList>();
            gvm.groupList = new List<Group>();
            gs = db.GroupStructure.Where(a => a.groupStructureId == grId1).FirstOrDefault();
            string[] extractingAcademicYear = new string[1];
            extractingAcademicYear = gs.academicQuarter.Split('-');
            gvm.studentList = new List<Student>();
            string acadYear = extractingAcademicYear[0];
            var stdList = db.Student.Where(a => a.sectionName == gs.section.sectionName && a.academicYearId == acadYear).ToList();
            foreach (var ks in stdList)
            {
                var tempGroupMember = db.StudentGroupList.Where(a => a.studentId == ks.studentId).FirstOrDefault();
                if (tempGroupMember == null)
                {

                    gvm.studentList.Add(ks);
                }
            }
         
           
            gvm.groupList = db.Group.Where(a => a.groupStrId == gs.groupStructureId).ToList();
            gvm.groupMembers = db.StudentGroupList.ToList();
            gvm.maxMembers = gs.maxNumberOfMembers;
            gvm.minMembers = gs.minNumberOfMembers;

            return View(gvm);
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

            

            return View();
        }

        public ActionResult AddGrade()
        {

            return PartialView("AddGrade");
        }
    }
}
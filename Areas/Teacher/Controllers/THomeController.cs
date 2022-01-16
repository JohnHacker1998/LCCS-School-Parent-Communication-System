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
    [Authorize(Roles = "Teacher")]
    
    public class THomeController : Controller
    {
        // GET: Teacher/THome
        public ActionResult Index()
        {
            return View();
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
                    ass.groupList = String.Join("##", theLists);
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
    }
}
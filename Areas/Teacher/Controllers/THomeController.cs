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
        [Authorize(Roles ="Teacher,HomeRoom,UnitLeader")]
        
        public ActionResult addAssignment()
        {
            //declaration
            //getting the current teacher  ID
            string currentID = User.Identity.GetUserId();
            Models.Teacher t = new Models.Teacher();
            HomeroomTeacherMethod ht = new HomeroomTeacherMethod();
            AcademicYear temp = new AcademicYear();
            ApplicationDbContext db = new ApplicationDbContext();
            addIndividualAssignmentViewModel avm = new addIndividualAssignmentViewModel();
            avm.studentList = new List<Student>();
            ViewBag.cannothaveSections = " ";

            List<Section> s = new List<Section>();
            avm.listAssignment = new List<Assignment>();
            avm.listAssignment = db.Assignment.ToList();
            
            avm.sectionList = new List<string>();
            t = db.Teacher.Where(a => a.teacherId == currentID).FirstOrDefault();
            string teacherGrade = t.grade.ToString();
            //getting all the sections with the teacher grade
            s = db.Section.Where(a => a.sectionName.StartsWith(teacherGrade)).ToList();
            string currentAcademicYearName = "";
            //finding out the current academic year
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
            //checking the current days appearance on the quarters time span
            //adding the active sections of the teacher existing on the current academic year
           if (quarter == "Q1" || quarter == "Q2" || quarter == "Q3" || quarter == "Q4") {
                var currentSections = db.Section.Where(a => a.academicYearId == currentAcademicYearName && a.sectionName.StartsWith(teacherGrade)).ToList();
                foreach (var k in currentSections)
                {
                    avm.sectionList.Add(k.sectionName);
                }
                avm.studentList = db.Student.Where(a => a.academicYearId == currentAcademicYearName).ToList();
                string acQuarter = currentAcademicYearName + "-" + quarter;

                
            }
            else
            {
                //error message for the view
                ViewBag.cannothaveSections="This date doesn't exist in the current Academic Year";
            }
            

            return View(avm);
        }
        [Authorize(Roles = "Teacher,HomeRoom,UnitLeader")]
        public bool fulfillsPercentage(string yearlyQuarter, int sectionID, string teacherID, int percentage)
        {
            //declaring variables and objects
            ApplicationDbContext db = new ApplicationDbContext();
            List <Assignment> listAssignment= new List<Assignment>();
            var theTeacher = db.Teacher.Where(ax => ax.teacherId == teacherID).FirstOrDefault();
            listAssignment = db.Assignment.Where(ax => ax.yearlyQuarter == yearlyQuarter && ax.sectionID == sectionID && ax.teacherId == teacherID).ToList();
            List<Schedule> sch = new List<Schedule>();
            sch = db.Schedule.Where(ax => ax.academicYear == yearlyQuarter && ax.grade == theTeacher.grade && ax.subject == theTeacher.subject).ToList();
            int sum = 0;
            //adding the mark percentage of the assignments existing on the academic quarter of the teacher 
            if (listAssignment != null) { 
            foreach(var k in listAssignment)
            {
                    sum += k.markPercentage;
            }
            }
            //adding up the scheduled assignments mark percentage
            if (sch != null)
            {
                foreach(var k in sch)
                {
                    sum += k.percentage;
                }
            }
            //if the sum is greater 100, returning false but if it is less returning true
            if (sum + percentage <= 100)
            {
                return true;
            }
            return false;
        }
        [Authorize(Roles = "Teacher,HomeRoom,UnitLeader")]
        [HttpPost]
        public ActionResult addAssignment(addIndividualAssignmentViewModel avm, HttpPostedFileBase file, string assignmentType, string sectionName, string fileName)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            HomeroomTeacherMethod ht = new HomeroomTeacherMethod();
            Assignment a = new Assignment();
            Models.Teacher t = new Models.Teacher();

            AcademicYear temp = new AcademicYear();
            assignmentViewModel avm2 = new assignmentViewModel();
            avm.studentList = new List<Student>();
            avm2.listAssignment = new List<Assignment>();
            avm2.gsList = new List<GroupStructure>();


            List<Section> s = new List<Section>();


            ViewBag.selectedType = " ";
            ViewBag.selectedSection = " ";
            ViewBag.successfulMessage = " ";
            ViewBag.failedMessage = " ";
            ViewBag.doesntFulfillMessage = " ";
            ViewBag.assignmentExists = " ";
            ViewBag.tudayerror = " ";
            ViewBag.sectionnull = " ";
            ViewBag.incorrectFIleType = " ";
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
            if (sectionName != null) { 
            var theSection = db.Section.Where(g => g.sectionId == currentSectionID).FirstOrDefault();
            string currentQuarter = ht.whichQuarter(theSection.academicYearId);
            if (ModelState.IsValid) {
                if (currentSectionID != 0 && ht.beforeQuarterEnd(Convert.ToDateTime(avm.submissionDate), theSection.academicYearId) && (currentQuarter == "Q1" || currentQuarter == "Q2" || currentQuarter == "Q3" || currentQuarter == "Q4") && fulfillsPercentage(theSection.academicYearId + "-" + currentQuarter, currentSectionID, currentTeacherID, avm.markPercentage) && file.ContentType== "application/pdf")
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
                    var assx = db.Assignment.Where(ax => ax.yearlyQuarter == a.yearlyQuarter && ax.assignmentType == "Individual" && ax.sectionID == a.sectionID && ax.assignmentName == a.assignmentName).ToList();
                    if (assx.Count() == 0)
                    {
                        db.Assignment.Add(a);
                        db.SaveChanges();
                        ViewBag.successfulMessage = "Assignment added Successfully.";
                        avm2.listAssignment = db.Assignment.Where(x => x.teacherId == currentTeacherID && x.yearlyQuarter == a.yearlyQuarter).ToList();
                        avm2.gsList = db.GroupStructure.Where(x => x.teacherId == currentTeacherID && x.academicQuarter == a.yearlyQuarter && x.completeStatus == 1).ToList();
                        return View("assignmentManagement", avm2);
                    }
                    else
                    {
                        ViewBag.assignmentExists = "Assignment already exists";
                    }
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
                        ViewBag.doesntFulfillMessage = "Mark Percentage sum is above 100";
                    }
                    if (ht.beforeQuarterEnd(Convert.ToDateTime(avm.submissionDate), theSection.academicYearId) == false)
                    {
                        ViewBag.tudayerror = "Submission Date is out of academic quarter";
                    }
                        if (file.ContentType != "application/pdf")
                        {
                            ViewBag.incorrectFIleType = "Invalid file format, please upload pdf file.";
                        }


                    }
            }
        }
            else
            {
                ViewBag.sectionnull = "Sections don't exist";
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
            avm.studentList = db.Student.Where(ab => ab.academicYearId == currentAcademicYearName && ab.sectionName.StartsWith(teacherGrade)).ToList();
            foreach (var k in currentSections)
            {
                avm.sectionList.Add(k.sectionName);
            }

            return View(avm);

        }
        [Authorize(Roles = "Teacher,HomeRoom,UnitLeader")]
        [HttpGet]
        public ActionResult addGroupAssignment(string gsId,string txtAdd)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            addGroupAssignmentViewModel avm = new addGroupAssignmentViewModel();
            avm.groupList = new List<Group>();
            int gsId1 = int.Parse(txtAdd);
            GroupStructure gs = new GroupStructure();
            gs = db.GroupStructure.Where(ax=>ax.groupStructureId==gsId1).FirstOrDefault();
           
            avm.groupList= db.Group.Where(ax=>ax.groupStrId==gsId1).ToList();
            avm.groupStructureId = gs.groupStructureId;

            return View(avm);
        }
        [Authorize(Roles = "Teacher,HomeRoom,UnitLeader")]
        [HttpPost]
        public ActionResult addGroupAssignment(string selectedStudents,addGroupAssignmentViewModel avm, string add, HttpPostedFileBase file,string fileName)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            GroupStructure gs = new GroupStructure();
            HomeroomTeacherMethod ht = new HomeroomTeacherMethod();
            assignmentViewModel avm2 = new assignmentViewModel();
            GroupStructureAssignment gsa = new GroupStructureAssignment();
            Assignment ass = new Assignment();
            List<Group> gList = new List<Group>();

            ViewBag.assignmentExists = " ";
            ViewBag.OutOfPercentage = " ";
            ViewBag.tudayerror = " ";
            ViewBag.failedMessage = " ";
            ViewBag.incorrectFIleType = " ";

            //GroupStructure gs
            string teacherID = User.Identity.GetUserId();
            Models.Teacher t = new Models.Teacher();
            t = db.Teacher.Where(ax => ax.teacherId == teacherID).FirstOrDefault();
           
            gs = db.GroupStructure.Where(ax => ax.groupStructureId == avm.groupStructureId).FirstOrDefault();
            string[] arr = new string[1];
            arr = gs.academicQuarter.Split('-');
            string aYear = arr[0];
            var currentSect = db.Section.Where(ax => ax.sectionId == gs.sectionId).FirstOrDefault();
            string currentQuarter = ht.whichQuarter(currentSect.academicYearId);

            if (add != null)
            {
                if (ModelState.IsValid) { 

                if (fulfillsPercentage(gs.academicQuarter, gs.sectionId, teacherID, avm.markPercentage) == true && ht.beforeQuarterEnd(Convert.ToDateTime(avm.submissionDate), aYear) && (currentQuarter == "Q1" || currentQuarter == "Q2" || currentQuarter == "Q3" || currentQuarter == "Q4") && file.ContentType== "application/pdf")
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
                        var assx = db.Assignment.Where(ax => ax.yearlyQuarter == ass.yearlyQuarter && ax.assignmentType == "Group" && ax.sectionID == ass.sectionID && ax.assignmentName == ass.assignmentName).ToList();
                        if (assx.Count()==0)
                        {
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

                                foreach (var n in memList)
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
                            avm2.gsList = new List<GroupStructure>();
                            avm2.gsList = db.GroupStructure.Where(ax => ax.teacherId == teacherID && ax.academicQuarter == gs.academicQuarter && ax.completeStatus == 1 && ax.section.sectionName.StartsWith(teacherGrade)).ToList();
                            avm2.listAssignment = db.Assignment.Where(ax => ax.teacherId == teacherID && ax.yearlyQuarter == gs.academicQuarter).ToList();
                            return View("assignmentManagement", avm2);
                        }
                        else
                        {
                            ViewBag.assignmentExists = "Assignment already exists";
                        }
                 
                }
                    else
                    {
                        if(fulfillsPercentage(gs.academicQuarter, gs.sectionId, teacherID, avm.markPercentage) == false)
                        {
                            ViewBag.OutOfPercentage = "Mark Percentage sum is above 100";
                        }
                        if(ht.beforeQuarterEnd(Convert.ToDateTime(avm.submissionDate), aYear) == false)
                        {
                            ViewBag.tudayerror = "Submission Date is out of academic quarter";
                        }
                        if (file.ContentType != "application/pdf")
                        {
                            ViewBag.incorrectFIleType = "Invalid file format, please upload pdf file.";
                        }
             
                    }
            }
            }
            avm.groupList = db.Group.Where(ax => ax.groupStrId == gs.groupStructureId).ToList();
            avm.groupStructureId = gs.groupStructureId;

            return View(avm);
        }
        [Authorize(Roles = "Teacher,HomeRoom,UnitLeader")]
        [HttpGet]
        public ActionResult updateAssignment(string assId, string txtAssignmentID)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            int assId1 = int.Parse(txtAssignmentID);
            HomeroomTeacherMethod ht = new HomeroomTeacherMethod();
            Assignment ass = new Assignment();
            ass = db.Assignment.Where(ax => ax.assignmentId == assId1).FirstOrDefault();
            updateAssignmentViewModel avm = new updateAssignmentViewModel();
           
            avm.assignmentID = ass.assignmentId;
            avm.assignmentName = ass.assignmentName;
            avm.markPercentage = ass.markPercentage;
            avm.submissionDate = ass.submissionDate.ToShortDateString();
            
            return View(avm);
        }
        [Authorize(Roles = "Teacher,HomeRoom,UnitLeader")]
        [HttpPost]
        public ActionResult updateAssignment(string fileName, updateAssignmentViewModel avm, HttpPostedFileBase file)
        {
            string teacherID = User.Identity.GetUserId();
            ApplicationDbContext db = new ApplicationDbContext();
            HomeroomTeacherMethod ht = new HomeroomTeacherMethod();
            assignmentViewModel avm2 = new assignmentViewModel();
            Assignment ass = new Assignment();
            ViewBag.OutOfPercentage = " ";
            ViewBag.tudayerror = " ";
            ViewBag.AlreadyExists = " ";
            ViewBag.incorrectFIleType = " ";
            avm2.listAssignment = new List<Assignment>();
            Models.Teacher t = new Models.Teacher();
            t = db.Teacher.Where(ax => ax.teacherId == teacherID).FirstOrDefault();
            string teacherGrade = t.grade.ToString();
            avm2.gsList = new List<GroupStructure>();
            ass = db.Assignment.Where(ax => ax.assignmentId == avm.assignmentID).FirstOrDefault();
            string[] arr = new string[1];
            arr = ass.yearlyQuarter.Split('-');
            string aYear = arr[0];
            if (ModelState.IsValid) {
                int currentSum = 0; 
                if (avm.markPercentage != ass.markPercentage) { 
                 currentSum = avm.markPercentage-ass.markPercentage;
                }
                else
                {
                    currentSum = 0;
                }
                if (fulfillsPercentage(ass.yearlyQuarter, ass.sectionID, ass.teacherId, currentSum) == true && ht.beforeQuarterEnd(Convert.ToDateTime(avm.submissionDate), aYear) && ((file!=null && file.ContentType== "application/pdf") || file==null))
            {
                ass.assignmentName = avm.assignmentName;
                ass.submissionDate = Convert.ToDateTime(avm.submissionDate).Date;
                ass.markPercentage = avm.markPercentage;
                if (fileName != null && fileName != "")
                {
                    int length = file.ContentLength;
                    byte[] upload = new byte[length];
                    file.InputStream.Read(upload, 0, length);
                    ass.assignmentDocument = upload;
                    ass.fileName = fileName;
                       
                    }
              /*  var assx = db.Assignment.Where(ax => ax.yearlyQuarter == ass.yearlyQuarter && ax.assignmentType == "Group" && ax.sectionID == ass.sectionID && ax.assignmentName == ass.assignmentName && ax.markPercentage==ass.markPercentage).ToList();
                if (assx.Count() == 0) {*/
                    db.SaveChanges();
                    avm2.gsList = db.GroupStructure.Where(ax => ax.teacherId == teacherID && ax.academicQuarter == ass.yearlyQuarter && ax.completeStatus == 1 && ax.section.sectionName.StartsWith(teacherGrade)).ToList();
                    avm2.listAssignment = db.Assignment.Where(ax => ax.teacherId == teacherID && ax.yearlyQuarter == ass.yearlyQuarter).ToList();
                    return View("assignmentManagement", avm2);
              

            }
                else
                {
                    if (fulfillsPercentage(ass.yearlyQuarter, ass.sectionID, ass.teacherId,currentSum) == false)
                    {
                        ViewBag.OutOfPercentage = "Mark Percentage sum is above 100";
                    }
                    if (ht.beforeQuarterEnd(Convert.ToDateTime(avm.submissionDate), aYear) == false)
                    {
                        ViewBag.tudayerror = "Submission Date is out of academic quarter";
                    }
                    if (file.ContentType != "application/pdf")
                    {
                        ViewBag.incorrectFIleType = "Invalid file format, please upload pdf file.";
                    }

                }
        }

            return View(avm);
        }
        [Authorize(Roles = "Teacher,HomeRoom,UnitLeader")]
        public ActionResult assignmentManagement()
        {
           
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
            avm.listAssignment = new List<Assignment>();
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
            
            if (quarter == "Q1" || quarter == "Q2" || quarter == "Q3" || quarter == "Q4")
            {
                string academicQuarter = currentAcademicYearName + "-" + quarter;
                avm.gsList = db.GroupStructure.Where(gr => gr.academicQuarter == academicQuarter && gr.section.sectionName.StartsWith(teacherGrade) && gr.completeStatus == 1).ToList();
                avm.listAssignment = db.Assignment.Where(ax => ax.yearlyQuarter == academicQuarter && ax.teacherId == currentID).ToList();
            }
            
            
            


                return View(avm);
        }
        [Authorize(Roles = "Teacher,HomeRoom,UnitLeader")]

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
            AcademicYear temp1 = new AcademicYear();
            List<Section> s = new List<Section>();
            avm.gsList = new List<GroupStructure>();
            avm.listAssignment = new List<Assignment>();

            int assId1 = int.Parse(assId);
            a = db.Assignment.Where(b => b.assignmentId == assId1).FirstOrDefault();

            if (delete != null)
            {
                if(!(Object.ReferenceEquals(a, null))) { 
                
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

            }
            s = db.Section.Where(ax => ax.sectionName.StartsWith(teacherGrade)).ToList();
            string currentAcademicYearName = "";

            foreach (var k in s)
            {
                if (ht.isInAcademicYear(k.academicYearId))
                {
                    currentAcademicYearName = k.academicYearId;
                    break;
                }
            }
            temp1 = db.AcademicYear.Where(ax => ax.academicYearName == currentAcademicYearName).FirstOrDefault();
            string quarter = ht.whichQuarter(currentAcademicYearName);
           
            if (quarter == "Q1" || quarter == "Q2" || quarter == "Q3" || quarter == "Q4")
            {
                string academicQuarter = currentAcademicYearName + "-" + quarter;
                avm.gsList = db.GroupStructure.Where(ax => ax.teacherId == teacherID && ax.academicQuarter == academicQuarter && ax.completeStatus == 1 && ax.section.sectionName.StartsWith(teacherGrade)).ToList();
                avm.listAssignment = db.Assignment.Where(ax => ax.teacherId == teacherID && ax.yearlyQuarter == academicQuarter).ToList();
            }
            
           

            return View(avm);
        }
        [Authorize(Roles = "Teacher,HomeRoom,UnitLeader")]

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
                gsvm.groupList = db.GroupStructure.Where(ax => ax.academicQuarter == acQuarter && ax.section.sectionName.StartsWith(teacherGrade) &&  ax.teacherId==currentID).ToList();
            }
            return View(gsvm);
        }
        [Authorize(Roles = "Teacher,HomeRoom,UnitLeader")]
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
                if (!(Object.ReferenceEquals(gs, null)))
                { 
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
                gsvm.groupList = db.GroupStructure.Where(ax => ax.academicQuarter == gs.academicQuarter && ax.section.sectionName.StartsWith(teacherGrade) && ax.teacherId==teacherId ).ToList();
                }
            }
            return View(gsvm);
        }
        [Authorize(Roles = "Teacher,HomeRoom,UnitLeader")]
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
            var currentSections = db.Section.Where(a => a.academicYearId == currentAcademicYearName && a.sectionName.StartsWith(teacherGrade)).ToList();

            foreach (var k in currentSections)
            {

                gsvm.sectionList.Add(k.sectionName);
            }
            gsvm.academicQuarter = currentAcademicYearName + "-" + quarter;

            return View(gsvm);
        }
        [Authorize(Roles = "Teacher,HomeRoom,UnitLeader")]
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
            ViewBag.existsMessage = " ";
            ViewBag.maximumMembers = " ";
            string extractedAcademicYear = gvm.academicQuarter.Substring(0, gvm.academicQuarter.IndexOf('-'));

           
            if (ModelState.IsValid) {

                s = db.Section.Where(a => a.sectionName == selectSections && a.academicYearId == extractedAcademicYear).FirstOrDefault();

                int std = db.Student.Where(ax => ax.sectionName == s.sectionName && ax.academicYearId == s.academicYearId).Count();
                if (gvm.maxNumberOfMembers <= std && gvm.minNumberOfMembers<=std && gvm.minNumberOfMembers<=gvm.maxNumberOfMembers) { 
                gs.academicQuarter = gvm.academicQuarter;
                gs.groupStructureName = gvm.groupStructureName;
                gs.maxNumberOfMembers = gvm.maxNumberOfMembers;
                gs.minNumberOfMembers = gvm.minNumberOfMembers;
                gs.sectionId = s.sectionId;
                gs.teacherId = currentID;
                var grS = db.GroupStructure.Where(ax => ax.academicQuarter == gs.academicQuarter && ax.groupStructureName == gs.groupStructureName && ax.minNumberOfMembers==gs.minNumberOfMembers && ax.maxNumberOfMembers==gs.maxNumberOfMembers).FirstOrDefault();
                if (grS == null) {
                    db.GroupStructure.Add(gs);
                    db.SaveChanges();
                    gvm.groupList = db.GroupStructure.Where(gr => gr.academicQuarter == gvm.academicQuarter && gr.section.sectionName.StartsWith(teacherGrade) && gr.teacherId == currentID).ToList();
                    return View("manageGroup", gvm);
                }
                else
                {
                    ViewBag.existsMessage = "Similar group structure exists";
                }
                }
                else
                {
                    if (gvm.maxNumberOfMembers >std) { 
                    ViewBag.maximumMembers = "The max number of members is out of bounds";
                    }
                    if (gvm.minNumberOfMembers > std)
                    {
                        ViewBag.maximumMembers = "The minimum number of members is out of bounds";
                    }
                    if (gvm.minNumberOfMembers > gvm.maxNumberOfMembers)
                    {
                        ViewBag.maximumMembers = "The minimum number of members is greater than maximum number of members";
                    }
                }

            }
            sec = db.Section.Where(a => a.sectionName.StartsWith(teacherGrade)).ToList();
            var currentSections = db.Section.Where(a => a.academicYearId == extractedAcademicYear && a.sectionName.StartsWith(teacherGrade)).ToList();

            foreach (var k in currentSections)
            {

                gvm.sectionList.Add(k.sectionName);
            }
            return View(gvm);
        }
        [Authorize(Roles = "Teacher,HomeRoom,UnitLeader")]
        [HttpGet]
        public ActionResult classifyGroup(string grIdOne,string txtImportant)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            classifyGroupViewModel gvm = new classifyGroupViewModel();
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
                var tempGroupMember = db.StudentGroupList.Where(a => a.studentId == ks.studentId).ToList();

                
                if (tempGroupMember == null)
                {
                    
                    gvm.studentList.Add(ks);
                }
                else if (tempGroupMember != null)
                {
                    int count = 0;
                    foreach(var n in tempGroupMember)
                    {
                        var x = db.Group.Where(ax => ax.groupId == n.groupId && ax.groupStrId == gs.groupStructureId).FirstOrDefault();
                        if (x == null)
                        {

                            count++;
                        }

                    }
                    if (count == tempGroupMember.Count()) {
                    gvm.studentList.Add(ks);
                    }
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
        [Authorize(Roles = "Teacher,HomeRoom,UnitLeader")]
        [HttpPost]
        public ActionResult classifyGroup(string grID,string skr,classifyGroupViewModel gvm, string txtValue,string add,string delete, string btnSubmitGrouping)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Models.Teacher t = new Models.Teacher();
            ViewBag.ErrorMembers = " ";
            ViewBag.successMessage = " ";
            ViewBag.ErrorExists = " ";
            ViewBag.enableMultiGroup = false;
            ViewBag.ErrorExists2 = " ";
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
                if (ModelState.IsValid) { 

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
                        if (tempG != null)
                        {
                            ViewBag.ErrorExists = "Group Name Exists.";
                        }
                    
                    if(txtValue == "*")
                        {
                            ViewBag.ErrorExists2 = "Please select students to add";
                        }
                }
            }
        }
            else if (delete != null)
            {
                int grID1 = int.Parse(grID);
                g = db.Group.Where(gr => gr.groupId == grID1).FirstOrDefault();
                if(!(Object.ReferenceEquals(g, null))) { 
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
            foreach (var ks in tempStudentList)
            {
                var tempGroupMember = db.StudentGroupList.Where(a => a.studentId == ks.studentId).ToList();


                if (tempGroupMember == null)
                {

                    gvm.studentList.Add(ks);
                }
                else if (tempGroupMember != null)
                {
                    int count = 0;
                    foreach (var n in tempGroupMember)
                    {
                        var x = db.Group.Where(ax => ax.groupId == n.groupId && ax.groupStrId == gs.groupStructureId).FirstOrDefault();
                        if (x == null)
                        {

                            count++;
                        }

                    }
                    if (count == tempGroupMember.Count())
                    {
                        gvm.studentList.Add(ks);
                    }
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
        [Authorize(Roles = "Teacher,HomeRoom,UnitLeader")]
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
        [Authorize(Roles = "Teacher,HomeRoom,UnitLeader")]
        public ActionResult GradeManagement()
        {
            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            ApplicationDbContext contextResult = new ApplicationDbContext();
            StudentViewModel studentViewModel = new StudentViewModel();
            studentViewModel.student = new List<Student>();
            HomeroomTeacherMethod homeroomTeacherMethod = new HomeroomTeacherMethod();
            Result result = new Result();

            //get teacher information
            string tId = User.Identity.GetUserId().ToString();
            var teacher = context.Teacher.Find(tId);

            //get active acadamic year
            var allAcadamicYears = context.AcademicYear.ToList();

            if (allAcadamicYears.Count != 0)
            {
                foreach (var getAcadamicYear in allAcadamicYears)
                {
                    //check today is in between start and end date of the specific academic year
                    if (!(DateTime.Compare(DateTime.Now, getAcadamicYear.durationStart) < 0 || DateTime.Compare(DateTime.Now, getAcadamicYear.durationEnd) > 0))
                    {
                        //get section
                        var listOfSections = context.Section.Where(s => s.academicYearId == getAcadamicYear.academicYearName && s.sectionName.StartsWith(teacher.grade.ToString())).ToList();
                        if (listOfSections.Count > 0)
                        {
                            foreach (var getSection in listOfSections)
                            {
                                //get students in that section
                                var studentinSection = context.Student.Where(s=>s.sectionName==getSection.sectionName && s.academicYearId==getAcadamicYear.academicYearName).ToList();

                                if (studentinSection.Count != 0)
                                {
                                    foreach (var getStudents in studentinSection)
                                    {
                                        studentViewModel.student.Add(getStudents);

                                        //check student is absent on assesement date without reason
                                        var quarter = homeroomTeacherMethod.whichQuarter(getAcadamicYear.academicYearName);
                                        var absentCheck = context.AbsenceRecord.Where(a=>a.studentId==getStudents.studentId && a.academicPeriod==getAcadamicYear.academicYearName+"-"+quarter && a.evidenceFlag==null).ToList();

                                        if (absentCheck.Count!=0)
                                        {
                                            foreach (var getAbsent in absentCheck)
                                            {
                                                //get schedule and assignment record informations
                                                var schedules = context.Schedule.Where(s => s.academicYear == getAcadamicYear.academicYearName + "-" + quarter && s.subject == teacher.subject && s.grade == teacher.grade && s.scheduleDate==getAbsent.recordDate.Date).FirstOrDefault();
                                                var assignments = context.Assignment.Where(a => a.yearlyQuarter == getAcadamicYear.academicYearName + "-" + quarter && a.teacher.subject == teacher.subject && a.teacher.grade == teacher.grade && a.submissionDate == getAbsent.recordDate.Date).FirstOrDefault();

                                                //check if absence exist on scheduled assesement date
                                                if (schedules!=null)
                                                {
                                                    //check if it is non evidence absence
                                                    getAbsent.recordDate = getAbsent.recordDate.Add(TimeSpan.FromDays(1));

                                                    if (DateTime.Compare(DateTime.Now.Date,getAbsent.recordDate.Date)>0)
                                                    {
                                                        AbsenceRecord present = new AbsenceRecord();
                                                        int count = 0;

                                                        do
                                                        {
                                                            present = context.AbsenceRecord.Where(a=>a.recordDate==getAbsent.recordDate.Date).FirstOrDefault();
                                                            getAbsent.recordDate = getAbsent.recordDate.Add(TimeSpan.FromDays(1));
                                                            if(present == null)
                                                            {
                                                                count++;
                                                            }
                                                        }
                                                        while (present!=null && DateTime.Compare(DateTime.Now.Date, getAbsent.recordDate.Date) > 0);

                                                        if(count==1 && DateTime.Compare(DateTime.Now.Date, getAbsent.recordDate.Date) >= 0)
                                                        {
                                                            //give the student zero for that assesement
                                                            var resultDuplicate = context.Result.Where(r => r.studentId == getStudents.studentId && r.scheduleId == schedules.scheduleId).FirstOrDefault();
                                                            if (resultDuplicate == null)
                                                            {
                                                                result.teacherId = tId;
                                                                result.studentId = getStudents.studentId;
                                                                result.result = 0;
                                                                result.feedback = "Student is Absent On the Assesement Day";
                                                                result.scheduleId = schedules.scheduleId;
                                                                result.assignmentId = null;
                                                                result.resultFor = schedules.scheduleFor;
                                                                result.percent = schedules.percentage;
                                                                result.academicYear = schedules.academicYear;
                                                                result.grade = teacher.grade;

                                                                //save result
                                                                contextResult.Result.Add(result);
                                                                int success = contextResult.SaveChanges();
                                                            }

                                                        }
                                                    }
                                                }
                                                if (assignments != null)
                                                {
                                                    
                                                    getAbsent.recordDate = getAbsent.recordDate.Add(TimeSpan.FromDays(1));

                                                    //get absent dates
                                                    if (DateTime.Compare(DateTime.Now.Date, getAbsent.recordDate.Date) > 0)
                                                    {
                                                        AbsenceRecord present = new AbsenceRecord();
                                                        int count = 0;

                                                        do
                                                        {
                                                            present = context.AbsenceRecord.Where(a => a.recordDate == getAbsent.recordDate.Date).FirstOrDefault();
                                                            getAbsent.recordDate = getAbsent.recordDate.Add(TimeSpan.FromDays(1));
                                                            if (present == null)
                                                            {
                                                                count++;
                                                            }
                                                        }
                                                        while (present != null && DateTime.Compare(DateTime.Now.Date, getAbsent.recordDate.Date) > 0);

                                                        if (count == 1 && DateTime.Compare(DateTime.Now.Date, getAbsent.recordDate.Date) >= 0)
                                                        {
                                                            //save data here
                                                            var resultDuplicate = context.Result.Where(r => r.studentId == getStudents.studentId && r.assignmentId==assignments.assignmentId).FirstOrDefault();
                                                            if (resultDuplicate == null)
                                                            {
                                                                //populate result
                                                                result.teacherId = tId;
                                                                result.studentId = getStudents.studentId;
                                                                result.result = 0;
                                                                result.feedback = "Student is Absent On the Assesement Day";
                                                                result.scheduleId = null;
                                                                result.assignmentId = assignments.assignmentId;
                                                                result.resultFor = assignments.assignmentType;
                                                                result.percent = assignments.markPercentage;
                                                                result.academicYear = assignments.yearlyQuarter;
                                                                result.grade = teacher.grade;

                                                                //save result
                                                                contextResult.Result.Add(result);
                                                                int success = contextResult.SaveChanges();
                                                            }
                                                            
                                                       }
                                                    }
                                                }
                                            }
                                            
                                        }
                                    }
                                }                                
          
                            }
                        }
                    }
                }
            }

            return View(studentViewModel);
        }
        [Authorize(Roles = "Teacher,HomeRoom,UnitLeader")]

        public ActionResult AddGrade(string id)
        {
            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            HomeroomTeacherMethod homeroomTeacherMethod = new HomeroomTeacherMethod();
            AddGradeModal addGradeModal = new AddGradeModal();
            addGradeModal.dates = new List<string>();

            //get teacher information
            string tId = User.Identity.GetUserId().ToString();
            var teacher = context.Teacher.Find(tId);
            addGradeModal.studentId = int.Parse(id);

            int count = 0;

            Section identifyYear = new Section();

            //get acadamic year
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

            //get acadamic year and quarter
            var getYear = context.AcademicYear.Find(identifyYear.academicYearId);
            var quarter = homeroomTeacherMethod.whichQuarter(identifyYear.academicYearId);
            
            var schedules = context.Schedule.Where(s=>s.academicYear==getYear.academicYearName+"-"+quarter && (s.subject==teacher.subject || s.subject=="All") && s.grade==teacher.grade).ToList();

            if (schedules.Count != 0)
            {
                foreach(var getSchedule in schedules)
                {
                    var checkReassesement = context.AbsenceRecord.Where(a => a.studentId == addGradeModal.studentId && a.recordDate == getSchedule.scheduleDate.Date && a.evidenceFlag== "AcceptableReason").FirstOrDefault();
                    if (checkReassesement != null)
                    {
                        count++;
                        break;
                    }
                }

                foreach (var getSchedule in schedules)
                {
                    if (DateTime.Compare(DateTime.Now.Date, getSchedule.scheduleDate.Date) > 0)
                    {
                        var duplicate = context.Result.Where(r => r.scheduleId == getSchedule.scheduleId && r.studentId == addGradeModal.studentId).FirstOrDefault();
                        var attendance = context.AbsenceRecord.Where(a => a.studentId == addGradeModal.studentId && a.recordDate == getSchedule.scheduleDate.Date).FirstOrDefault();
                        if (duplicate == null && attendance==null)
                        {
                            if (getSchedule.scheduleFor!= "Reassessment")
                            {
                                addGradeModal.dates.Add(getSchedule.scheduleDate.ToShortDateString() + "-" + getSchedule.scheduleFor);
                            }
                            else
                            {
                                if (count != 0)
                                {
                                    addGradeModal.dates.Add(getSchedule.scheduleDate.ToShortDateString() + "-" + getSchedule.scheduleFor);
                                }
                            }
                            
                        }
                        
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
                        var duplicate = context.Result.Where(r => r.assignmentId == getAssignments.assignmentId && r.studentId == addGradeModal.studentId).FirstOrDefault();
                        var attendance = context.AbsenceRecord.Where(a => a.studentId == addGradeModal.studentId && a.recordDate == getAssignments.submissionDate.Date && a.evidenceFlag==null).FirstOrDefault();
                        if (duplicate == null && attendance==null)
                        {
                            addGradeModal.dates.Add(getAssignments.submissionDate.ToShortDateString() + "-" + getAssignments.assignmentType);
                        }
                        
                    }

                }
            }


            return PartialView("AddGrade",addGradeModal);
        }
        [Authorize(Roles = "Teacher,HomeRoom,UnitLeader")]

        [HttpPost]
        public ActionResult AddGrade(AddGradeModal addGradeModal,string dates)
        {

            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            Section identifyYear = new Section();
            HomeroomTeacherMethod homeroomTeacherMethod = new HomeroomTeacherMethod();
            Result result = new Result();

            string tId = User.Identity.GetUserId().ToString();
            var teacher = context.Teacher.Find(tId);
            string[] dateTaken = dates.Split('-');
            DateTime date = DateTime.Parse(dateTaken[0]).Date;

            //result for Continious Assessment Test & Final Exam

            if (dateTaken[1]== "Continious Assessment Test" || dateTaken[1]== "Final Exam")
            {
                var schedule = context.Schedule.Where(s=>s.scheduleDate==date && s.grade==teacher.grade && s.subject==teacher.subject).FirstOrDefault();
                var duplicate = context.Result.Where(r => r.scheduleId == schedule.scheduleId && r.studentId == addGradeModal.studentId).FirstOrDefault();

                if (duplicate == null)
                {
                    if (addGradeModal.result <= schedule.percentage)
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
                        //error result excced percentage
                        ViewBag.error = "Result Exceed the Score limit";
                    }
                }
                else
                {
                    //error message duplicate
                    ViewBag.error = "Result Already Exist!!";
                }
                    
            }
            else if (dateTaken[1]=="Group" || dateTaken[1]=="Individual")
            {
                var assignment = context.Assignment.Where(a=>a.submissionDate==date && a.teacher.grade==teacher.grade && a.teacher.subject==teacher.subject).FirstOrDefault();
                var attendance = context.AbsenceRecord.Where(a => a.studentId == addGradeModal.studentId && a.recordDate == date && a.evidenceFlag==null).FirstOrDefault();
                var duplicate = context.Result.Where(r => r.assignmentId == assignment.assignmentId && r.studentId == addGradeModal.studentId).FirstOrDefault();

                if (duplicate == null)
                {
                    if (addGradeModal.result <= assignment.markPercentage)
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
                        //error result excced percentage
                        ViewBag.error = "Result Exceed the Score limit";
                    }
                }
                else
                {
                    //error message duplicate
                    ViewBag.error = "Result Already Exist!!";
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
                var duplicate = context.Result.Where(r => r.scheduleId == reassessment.scheduleId && r.studentId == addGradeModal.studentId).FirstOrDefault();

                if (duplicate == null)
                {
                    if (schedule.Count != 0)
                    {
                        foreach (var getSchedules in schedule)
                        {
                            var getPercent = context.AbsenceRecord.Where(a => a.studentId == addGradeModal.studentId && a.academicPeriod == getYear.academicYearName + "-" + quarter && a.recordDate == getSchedules.scheduleDate && a.evidenceFlag == "AcceptableReason").FirstOrDefault();

                            if (getPercent != null)
                            {
                                sum += getSchedules.percentage;
                            }
                        }

                        if (sum != 0)
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
                else
                {
                    //error message duplicate
                    ViewBag.error = "Result Already Exist!!";
                }
                

            }

            return PartialView("AddGrade", addGradeModal);
        }
        [Authorize(Roles = "Teacher,HomeRoom,UnitLeader")]

        public ActionResult UpdateResultManagement(string id)
        {
            //get students previous current quarter same teacher

            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            UpdateResultViewModel updateResultViewModel = new UpdateResultViewModel();
            updateResultViewModel.results = new List<Result>();
            HomeroomTeacherMethod homeroomTeacherMethod = new HomeroomTeacherMethod();

            string tId = User.Identity.GetUserId().ToString();
            var teacher = context.Teacher.Find(tId);

            int Id = int.Parse(id);

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

            var results = context.Result.Where(r => r.teacherId == tId && r.academicYear == getYear.academicYearName + "-" + quarter && r.feedback != "Student is Absent On the Assesement Day" && r.studentId == Id).ToList();
            if (results.Count != 0)
            {
                foreach (var getResults in results)
                {
                    updateResultViewModel.results.Add(getResults);
                }
            }

            return View(updateResultViewModel);
        }
        [Authorize(Roles = "Teacher,HomeRoom,UnitLeader")]

        public ActionResult ResultUpdate(string id)
        {

            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            UpdateGradeModal updateGradeModal = new UpdateGradeModal();

            int rId = int.Parse(id);

            var result = context.Result.Find(rId);

            updateGradeModal.result = result.result;
            updateGradeModal.feedback = result.feedback;
            updateGradeModal.resultId = rId;

            return PartialView("ResultUpdate", updateGradeModal);
        }
        [Authorize(Roles = "Teacher,HomeRoom,UnitLeader")]

        [HttpPost]
        public ActionResult ResultUpdate(UpdateGradeModal updateGradeModal)
        {
            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();

            var result = context.Result.Find(updateGradeModal.resultId);

            if (updateGradeModal.result <= result.percent)
            {
                result.result = updateGradeModal.result;
                result.feedback = updateGradeModal.feedback;

                int success = context.SaveChanges();
                if (success > 0)
                {
                    ViewBag.complete = "Result Updated Successfully";
                }
                else
                {
                    ViewBag.error = "Failed to Update Result";
                }
            }
            else
            {
                ViewBag.error = "Result Exceed the Score limit";
            }

            return PartialView("ResultUpdate", updateGradeModal);

        }

        [Authorize(Roles = "HomeRoom")]
        public ActionResult addAttendance()
        {
            HomeroomTeacherMethod htm = new HomeroomTeacherMethod();
            ApplicationDbContext db = new ApplicationDbContext();
            AbsenceRecordViewMoel av = new AbsenceRecordViewMoel();
            AcademicYear ay = new AcademicYear();

            av.absenceList = new List<AbsenceRecord>();
            ViewBag.messageSuccessStatus = " ";
            //finding the logged on teacher ID
            string currentUserId = User.Identity.GetUserId();
            av.studentList = new List<Student>();
            //finding the student list of where the teacher is homeroom for
            var tempStdList = htm.getList(currentUserId);
            //counting number of students in the retrieved list
            if (tempStdList != null)
            {
                DateTime ks = DateTime.Now.Date;
                foreach (var k in tempStdList)
                {
                    AbsenceRecord abs = new AbsenceRecord();
                    abs = db.AbsenceRecord.Where(ax => ax.recordDate == ks && ax.studentId == k.studentId).FirstOrDefault();
                    if (abs == null)
                    {
                        av.studentList.Add(k);
                    }
                }
            }
            int a = av.studentList.Count();
            //creating an array with the size of the list of the students list to hold the selection on listbox
            DateTime recordDate1 = DateTime.Now.Date;
            Section s = new Section();

            //finding the section where the teacher is homeroom
            s = db.Section.Where(x => x.teacherId == currentUserId).FirstOrDefault();

            int i = 0;
            ay = db.AcademicYear.Where(r => r.academicYearName == s.academicYearId).FirstOrDefault();
            string status = htm.whichQuarter(s.academicYearId);

            List<AbsenceRecord> tempRecord1 = new List<AbsenceRecord>();

            ViewBag.Message = false;
            string academicRecord1 = s.academicYearId + "-" + status;

            //recording the current date as a short date string
            DateTime d = DateTime.Now.Date;
            av.absenceList = db.AbsenceRecord.Where(c => c.recordDate == d && c.recordDate == recordDate1 && c.academicPeriod == academicRecord1 && c.student.sectionName == s.sectionName).ToList();

            return View(av);
        }
        [Authorize(Roles = "HomeRoom")]
        [HttpPost]
        public ActionResult addAttendance(AbsenceRecordViewMoel arvm, string submit, string delete, string selectedStudents, string gsId)
        {
            HomeroomTeacherMethod htm = new HomeroomTeacherMethod();
            AbsenceRecord ar = new AbsenceRecord();
            ApplicationDbContext db = new ApplicationDbContext();
            Suspension suspension = new Suspension();
            arvm.absenceList = new List<AbsenceRecord>();
            string currentUserId = User.Identity.GetUserId();
            AcademicYear ay = new AcademicYear();
            arvm.studentList = htm.getList(currentUserId);
            Section s = new Section();
            s = db.Section.Where(x => x.teacherId == currentUserId).FirstOrDefault();
            ay = db.AcademicYear.Where(r => r.academicYearName == s.academicYearId).FirstOrDefault();
            string status = htm.whichQuarter(s.academicYearId);
            List<AbsenceRecord> tempRecord = new List<AbsenceRecord>();
            List<AbsenceRecord> tempRecord1 = new List<AbsenceRecord>();
            ViewBag.Message = false;
            string academicRecord1 = s.academicYearId + "-" + status;
            DateTime recordDate1 = DateTime.Now.Date;
            ViewBag.successfulMessage = " ";
            ViewBag.messageStatus = " ";


            if (submit != null)
            {


                if (selectedStudents != null && selectedStudents != "*")
                {
                    String firstSeries = selectedStudents.Substring(1, selectedStudents.Length - 2);

                    List<string> memList = new List<string>();
                    memList = firstSeries.Split('-').ToList();


                    foreach (var k in memList)
                    {

                        if (status == "Q1" || status == "Q2" || status == "Q3" || status == "Q4")
                        {
                            Student std = new Student();
                            std = db.Student.Where(ax => ax.fullName == k).FirstOrDefault();
                            ar.academicPeriod = s.academicYearId + "-" + status;
                            ar.recordDate = DateTime.Now.Date;
                            ar.studentId = std.studentId;

                            int count = htm.manageCount(ar.academicPeriod, std.studentId);
                            ar.count = count + 1;
                            db.AbsenceRecord.Add(ar);
                            db.SaveChanges();
                            ViewBag.successfulMessage = "Absence Record Added Successfully";


                        }
                        else
                        {
                            if (status == "no")
                            {
                                ViewBag.messageSuccessStatus = "Today doesn't exist in the academic year";
                            }
                            else if (status == "gap")
                            {
                                ViewBag.messageStatus = "Today is a gap";
                            }
                        }
                    }
                }

            }



            else if (delete != null)
            {

                int gsID1 = int.Parse(gsId);
                AbsenceRecord abs = new AbsenceRecord();
                abs = db.AbsenceRecord.Where(ax => ax.recordId == gsID1).FirstOrDefault();
                if (!(Object.ReferenceEquals(abs, null)))
                {
                    db.AbsenceRecord.Remove(abs);
                    db.SaveChanges();
                }
            }
            arvm.studentList = new List<Student>();

            var tempStdList = htm.getList(currentUserId);
            //counting number of students in the retrieved list
            if (tempStdList != null)
            {
                DateTime ks = DateTime.Now.Date;
                foreach (var k in tempStdList)
                {
                    AbsenceRecord abs = new AbsenceRecord();
                    abs = db.AbsenceRecord.Where(ax => ax.recordDate == ks && ax.studentId == k.studentId).FirstOrDefault();
                    if (abs == null)
                    {
                        arvm.studentList.Add(k);
                    }
                }
            }


            arvm.absenceList = new List<AbsenceRecord>();
            //recording the current date as a short date string
            DateTime d = DateTime.Now.Date;
            arvm.absenceList = db.AbsenceRecord.Where(c => c.recordDate == d && c.recordDate == recordDate1 && c.academicPeriod == academicRecord1 && c.student.sectionName == s.sectionName).ToList();
            return View(arvm);


        }
        [Authorize(Roles = "UnitLeader")]
        public ActionResult AddLateComer(string id)
        {

            LateComerViewModel lateComerViewModel = new LateComerViewModel();

            //check if current day is not weekend
            if (!(DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday))
            {
                //show conformation message
                lateComerViewModel.ID = Int32.Parse(id);
                ViewBag.message = "Are You Sure Do You Want To Declare the student As a Late Comer?";
            }
            else
            {
                //error message weekend
                ViewBag.error = "You are Not Allowed To Access on the Weekend";
            }

            return PartialView("AddLateComer", lateComerViewModel);
        }
        [Authorize(Roles = "UnitLeader")]
        [HttpPost]
        public ActionResult AddLateComer(LateComerViewModel lateComerViewModel)
        {

            //basic objects
            ApplicationDbContext context = new ApplicationDbContext();
            Suspension suspension = new Suspension();
            Collection collection = new Collection();
            LateComer lateComer = new LateComer();


            int count = 0;
            //get unitleader info
            string tId = User.Identity.GetUserId().ToString();
            var teacher = context.Teacher.Where(t => t.teacherId == tId).FirstOrDefault();

            //get how mach time the student is late and increment it by 1
            var lateRecord = context.LateComer.Where(l => l.studentId == lateComerViewModel.ID).ToList();
            if (lateRecord.Count != 0)
            {
                //get the late days count
                foreach (var getCount in lateRecord)
                {
                    if (count < getCount.dayCount)
                    {
                        count = getCount.dayCount;
                    }
                }
            }

            //check today is bounded in a quarter
            if (collection.currentQuarter(lateComerViewModel.ID) == " ")
            {
                //error message
                ViewBag.posterror = "Time is not Bounded in a Quarter";
            }
            else
            {
                //check if latecomer record exist for today
                DateTime currentDate = DateTime.Now.Date;
                var duplicate = context.LateComer.Where(l => l.lateDate == currentDate && l.studentId == lateComerViewModel.ID).FirstOrDefault();
                if (duplicate == null)
                {
                    //populate latecomer object
                    lateComer.academicPeriod = collection.currentQuarter(lateComerViewModel.ID);
                    lateComer.dayCount = count + 1;
                    lateComer.lateDate = DateTime.Now.Date;
                    lateComer.studentId = lateComerViewModel.ID;

                    //save latecomer record
                    context.LateComer.Add(lateComer);
                    int result = context.SaveChanges();

                    if (result > 0)
                    {
                        //send warning message if the counter reaches 3
                        if (lateComer.dayCount == 3 || lateComer.dayCount == 7 || lateComer.dayCount == 11)
                        {
                            //warning object
                            Warning warning = new Warning();

                            //populate warning object
                            warning.studentId = lateComerViewModel.ID;
                            warning.WarningReadStatus = "No";
                            warning.grade = teacher.grade;
                            warning.warningType = "LateComer";
                            warning.academicYear = collection.currentQuarter(lateComerViewModel.ID);
                            warning.warningDate = DateTime.Now.Date;

                            //save warning
                            context.Warning.Add(warning);
                            context.SaveChanges();
                        }
                        else if (lateComer.dayCount == 4 || lateComer.dayCount == 8 || lateComer.dayCount == 12)
                        {
                            //suspend student for two days
                            suspension.studentId = lateComerViewModel.ID;
                            suspension.startDate = DateTime.Now;
                            do
                            {
                                suspension.startDate = suspension.startDate.AddDays(1);
                            }
                            while (suspension.startDate.DayOfWeek == DayOfWeek.Saturday || suspension.startDate.DayOfWeek == DayOfWeek.Sunday);

                            suspension.endDate = suspension.startDate;
                            do
                            {
                                suspension.endDate = suspension.endDate.AddDays(1);
                            }
                            while (suspension.endDate.DayOfWeek == DayOfWeek.Saturday || suspension.endDate.DayOfWeek == DayOfWeek.Sunday);

                            //save suspension record
                            context.Suspension.Add(suspension);
                            context.SaveChanges();
                        }

                        //success message
                        ViewBag.complete = "Latecomer Recorded Successfully";
                    }
                    else
                    {
                        //error message
                        ViewBag.error = "Failed to Record Latecomer";
                    }
                }
                else
                {
                    //error message duplicate
                    ViewBag.error = "Student has Already been Labeled as a Latecomer.";
                }
            }


            return PartialView("AddLateComer");
        }

        [Authorize(Roles = "UnitLeader")]
        public ActionResult LateComerManagement()
        {

            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            Suspension suspension = new Suspension();
            LateComerViewModel lateComerViewModel = new LateComerViewModel();
            lateComerViewModel.students = new List<Student>();
            LateComer lateComer = new LateComer();
            Collection collection = new Collection();

            if (!(DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday))
            {

                //get unitleader info
                string tId = User.Identity.GetUserId().ToString();
                var teacher = context.Teacher.Where(t => t.teacherId == tId).FirstOrDefault();

                //get all academic years
                var academicYears = context.AcademicYear.ToList();
                foreach (var getActive in academicYears)
                {
                    //get start and end dates to check if today is in the middle
                    if (!(DateTime.Compare(DateTime.Now, getActive.durationStart) < 0 || DateTime.Compare(DateTime.Now, getActive.durationEnd) > 0))
                    {
                        //search student by grade in active academic years
                        lateComerViewModel.students = context.Student.Where(s => s.academicYearId == getActive.academicYearName && s.sectionName.StartsWith(teacher.grade.ToString())).ToList();
                    }
                }

                if (lateComerViewModel.students.Count != 0)
                {
                    //check if student is suspended or not
                    foreach (var checkStudent in lateComerViewModel.students)
                    {
                        var studentSuspended = context.Suspension.Where(s => s.studentId == checkStudent.studentId).FirstOrDefault();
                        if (studentSuspended != null)
                        {
                            //check if the suspention time elapsed
                            if (DateTime.Compare(studentSuspended.endDate, DateTime.Now) < 0)
                            {
                                //remove student from suspention table
                                context.Suspension.Remove(studentSuspended);
                                context.SaveChanges();
                            }
                            else
                            {
                                //remove student from search list
                                lateComerViewModel.students.Remove(checkStudent);
                            }
                        }
                    }

                }
            }
            else
            {
                //error message weekend
                ViewBag.message = "You are Not Allowed To Access on the Weekend";
            }

            return View(lateComerViewModel);
        }

        [Authorize(Roles = "UnitLeader")]
        public ActionResult SendWarning(string id)
        {
            WarningViewModel warningViewModel = new WarningViewModel();

            //check current day is not weekend
            if (!(DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday))
            {
                //conformation message
                warningViewModel.ID = Int32.Parse(id);
                ViewBag.message = "Are You Sure Do You Want To Send Warning Message For Selected Student?";
            }
            else
            {
                //error message weekend
                ViewBag.error = "You are Not Allowed To Access on the Weekend";
            }

            return PartialView("SendWarning", warningViewModel);
        }
        [Authorize(Roles = "UnitLeader")]

        [HttpPost]
        public ActionResult SendWarning(WarningViewModel warningViewModel)
        {
            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            Warning warningobj = new Warning();
            Collection collection = new Collection();

            string quarter = collection.currentQuarter(warningViewModel.ID);

            //get teacher grade from login info
            string tId = User.Identity.GetUserId().ToString();
            var teacher = context.Teacher.Where(t => t.teacherId == tId).FirstOrDefault();

            //populate warning object
            warningobj.academicYear = quarter;
            warningobj.warningDate = DateTime.Now.Date;
            warningobj.WarningReadStatus = "No";
            warningobj.warningType = "Atendance";
            warningobj.studentId = warningViewModel.ID;
            warningobj.grade = teacher.grade;

            context.Warning.Add(warningobj);
            int result = context.SaveChanges();
            if (result > 0)
            {
                //warning send message
                ViewBag.complete = "Warning Send Successfully";
            }
            else
            {
                //fail message
                ViewBag.posterror = "Failed to Send Warning!!";
            }

            return PartialView("SendWarning", warningViewModel);
        }
        [Authorize(Roles = "UnitLeader")]
        public ActionResult WarningManagement()
        {
            //object declaration
            WarningViewModel warningViewModel = new WarningViewModel();
            warningViewModel.eligible = new List<Student>();
            warningViewModel.nonViewed = new List<Warning>();

            //check current day is not weekend
            if (!(DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday))
            {
                //populate eligible and nonViewed objects
                warningViewModel = nonViewedWarnings();
                warningViewModel.eligible = eligibleStudents();

            }
            else
            {
                //error message weekend
                ViewBag.message = "You are Not Allowed To Access on the Weekend";
            }
            return View(warningViewModel);
        }
        [Authorize(Roles = "UnitLeader")]
        public ActionResult EvidenceApproval(int id)
        {
            //object declartion
            ApplicationDbContext context = new ApplicationDbContext();
            var evidenceApprovalViewModel = new EvidenceApprovalViewModel();
            evidenceApprovalViewModel.days = new List<string>();

            ViewBag.view = true;

            //search evidence record
            var evidence = context.Evidence.Find(id);

            DateTime yesterday = DateTime.Now.Date;

            //var yesterday = DateTime.Now.Subtract(TimeSpan.FromDays(1));

            do
            {
                yesterday = yesterday.Subtract(TimeSpan.FromDays(1));
            }
            while (yesterday.DayOfWeek == DayOfWeek.Saturday || yesterday.DayOfWeek == DayOfWeek.Sunday);

            //get all absence days for the specified evidence
            do
            {

                var daysRecord = context.AbsenceRecord.Where(a => a.studentId == evidence.parent.student.studentId && a.recordDate == yesterday.Date).FirstOrDefault();
                if (daysRecord != null)
                {
                    evidenceApprovalViewModel.days.Add(daysRecord.recordDate.Date.ToShortDateString());

                    do
                    {
                        yesterday = yesterday.Subtract(TimeSpan.FromDays(1));
                    }
                    while (yesterday.DayOfWeek == DayOfWeek.Saturday || yesterday.DayOfWeek == DayOfWeek.Sunday);
                }
                else
                {
                    break;
                }

            }
            while (true);

            evidenceApprovalViewModel.Id = id;
            evidenceApprovalViewModel.document = evidence.evidenceDocument;

            return View(evidenceApprovalViewModel);
        }
        [Authorize(Roles = "UnitLeader")]
        [HttpPost]
        public ActionResult EvidenceApproval(EvidenceApprovalViewModel evidenceApprovalViewModel, string[] days)
        {
            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            HomeroomTeacherMethod homeroomTeacherMethod = new HomeroomTeacherMethod();

            ViewBag.view = false;

            var evidence = context.Evidence.Find(evidenceApprovalViewModel.Id);
            evidence.approvalStatus = "Seen";

            context.SaveChanges();

            if (days != null)
            {
                for (int i = 0; i < days.Length; i++)
                {
                    DateTime date = DateTime.Parse(days[i]);
                    var absenceRecord = context.AbsenceRecord.Where(a => a.recordDate == date.Date && a.studentId == evidence.parent.student.studentId).FirstOrDefault();
                    absenceRecord.evidenceFlag = "AcceptableReason";
                    context.SaveChanges();
                }

                string quarter = homeroomTeacherMethod.whichQuarter(evidence.parent.student.academicYearId);
                var record = context.AbsenceRecord.Where(a => a.academicPeriod == evidence.parent.student.academicYearId + "-" + quarter && a.studentId == evidence.parent.studentId).ToList();

                record[record.Count - 1].count = record[record.Count - 1].count - days.Length;
                context.SaveChanges();
            }

            ViewBag.complete = "Task Completed Successfully";

            return View(evidenceApprovalViewModel);
        }
        [Authorize(Roles = "UnitLeader")]
        public ActionResult StudentEvidence()
        {
            //object declaration
            ApplicationDbContext context = new ApplicationDbContext();
            List<Student> students = new List<Student>();
            List<Evidence> evidence = new List<Evidence>();

            var currentDate = DateTime.Now.Date;

            //get teacher grade from login info
            string tId = User.Identity.GetUserId().ToString();
            var teacher = context.Teacher.Where(t => t.teacherId == tId).FirstOrDefault();

            //check current date is not weekend
            if (!(DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday))
            {
                //get evidence that parents send today
                evidence = context.Evidence.Where(e => e.parent.student.sectionName.StartsWith(teacher.grade.ToString()) && e.approvalStatus == "Provided" && e.dateUpload == currentDate).ToList();
            }
            else
            {
                //error message weekend
                ViewBag.message = "You are Not Allowed To Access on the Weekend";
            }


            return View(evidence);
        }

        [Authorize(Roles = "UnitLeader")]
        public ActionResult success()
        {

            return PartialView("success");
        }
        [Authorize(Roles = "UnitLeader")]

        public List<Student> eligibleStudents()
        {
            //basic objects
            ApplicationDbContext context = new ApplicationDbContext();
            Warning warning = new Warning();
            WarningViewModel warningViewModel = new WarningViewModel();
            HomeroomTeacherMethod homeroomTeacherMethod = new HomeroomTeacherMethod();
            warningViewModel.eligible = new List<Student>();

            //get teacher info
            string tId = User.Identity.GetUserId().ToString();
            var teacher = context.Teacher.Where(t => t.teacherId == tId).FirstOrDefault();

            //get all academic years
            var academicYears = context.AcademicYear.ToList();
            if (academicYears.Count > 0)
            {
                foreach (var getActive in academicYears)
                {
                    //get start and end dates to check if today is in the middle
                    if (!(DateTime.Compare(DateTime.Now, getActive.durationStart) < 0 || DateTime.Compare(DateTime.Now, getActive.durationEnd) > 0))
                    {
                        string quarter = homeroomTeacherMethod.whichQuarter(getActive.academicYearName);

                        //get students with three or more but less than 5 absence records in active academic years and under the unitleader management 
                        var students = context.AbsenceRecord.Where(a => a.academicPeriod == getActive.academicYearName + "-" + quarter && a.count >= 3 && a.count < 5 && a.student.sectionName.StartsWith(teacher.grade.ToString())).ToList();

                        if (students.Count != 0)
                        {
                            foreach (var getStudents in students)
                            {
                                //check prevoius day attendance to be sure about no evidence is provided
                                DateTime yesterday = DateTime.Now;
                                do
                                {
                                    yesterday = yesterday.Subtract(TimeSpan.FromDays(1));
                                }
                                while (yesterday.DayOfWeek == DayOfWeek.Sunday || yesterday.DayOfWeek == DayOfWeek.Saturday);
                                //string previousDate = yesterday.ToShortDateString();
                                var previousDay = context.AbsenceRecord.Where(a => a.recordDate == yesterday.Date && a.studentId == getStudents.studentId).FirstOrDefault();

                                if (previousDay == null)
                                {
                                    //check warning message is already their
                                    var inWarning = context.Warning.Where(w => w.academicYear == getActive.academicYearName + "-" + quarter && w.studentId == getStudents.studentId).FirstOrDefault();
                                    if (inWarning == null)
                                    {
                                        //populate the student as eligible for warning
                                        var studentEligible = context.Student.Where(s => s.studentId == getStudents.studentId).FirstOrDefault();
                                        warningViewModel.eligible.Add(studentEligible);
                                    }
                                }

                            }
                        }
                    }
                }
            }
            return warningViewModel.eligible;

        }
        [Authorize(Roles = "UnitLeader")]
        public WarningViewModel nonViewedWarnings()
        {
            //basic objects
            ApplicationDbContext context = new ApplicationDbContext();
            Warning warning = new Warning();
            WarningViewModel warningViewModel = new WarningViewModel();
            HomeroomTeacherMethod homeroomTeacherMethod = new HomeroomTeacherMethod();
            warningViewModel.nonViewed = new List<Warning>();
            warningViewModel.parentName = new List<string>();
            warningViewModel.parentPhone = new List<string>();

            //get teacher info
            string tId = User.Identity.GetUserId().ToString();
            var teacher = context.Teacher.Where(t => t.teacherId == tId).FirstOrDefault();

            //get all academic years
            var academicYears = context.AcademicYear.ToList();

            if (academicYears.Count > 0)
            {
                foreach (var getActive in academicYears)
                {
                    //get start and end dates to check if today is in the middle
                    if (!(DateTime.Compare(DateTime.Now, getActive.durationStart) < 0 || DateTime.Compare(DateTime.Now, getActive.durationEnd) > 0))
                    {
                        //get current quarter and students who register for that academic year who are managed by this unitleader
                        string quarter = homeroomTeacherMethod.whichQuarter(getActive.academicYearName);
                        var studentInGrade = context.Student.Where(s => s.sectionName.StartsWith(teacher.grade.ToString()) && s.academicYearId == getActive.academicYearName).ToList();

                        if (studentInGrade.Count != 0)
                        {
                            foreach (var getStudent in studentInGrade)
                            {
                                //get warnings that are not viewed by parents
                                var warnings = context.Warning.Where(w => w.WarningReadStatus == "No" && w.studentId == getStudent.studentId && w.academicYear == getActive.academicYearName + "-" + quarter).ToList();

                                if (warnings.Count != 0)
                                {
                                    //populate the nonviewed warnings to the viewmodel object

                                    foreach (var getWarnings in warnings)
                                    {
                                        warningViewModel.nonViewed.Add(getWarnings);

                                        //parent information
                                        var parent = context.Parent.Where(p => p.studentId == getStudent.studentId).ToList();

                                        if (parent.Count != 0)
                                        {
                                            if (parent.Count == 1)
                                            {
                                                warningViewModel.parentName.Add(parent[0].user.fullName);
                                                warningViewModel.parentPhone.Add(parent[0].user.PhoneNumber);
                                            }
                                            else if (parent.Count == 2)
                                            {
                                                warningViewModel.parentName.Add(parent[0].user.fullName + "/" + parent[1].user.fullName);
                                                warningViewModel.parentPhone.Add(parent[0].user.PhoneNumber + "/" + parent[1].user.PhoneNumber);
                                            }
                                        }
                                        else
                                        {
                                            warningViewModel.parentName.Add("Parent Not-Assigned");
                                            warningViewModel.parentPhone.Add("Parent Not-Assigned");
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
            return warningViewModel;
        }


    }
}
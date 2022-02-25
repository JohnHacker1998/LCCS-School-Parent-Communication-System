using LCCS_School_Parent_Communication_System.Identity;
using LCCS_School_Parent_Communication_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using LCCS_School_Parent_Communication_System.viewModels;
using LCCS_School_Parent_Communication_System.Additional_Class;

namespace LCCS_School_Parent_Communication_System.Areas.Homeroom_Teacher.Controllers
{
  //  [Authorize(Roles = "HomeRoom")]

    public class HTHomeController : Controller
    {

        // GET: Homeroom_Teacher/HTHome
        public ActionResult Index()
        {
            return View();
        }
       
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
                foreach(var k in tempStdList)
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
            av.absenceList = db.AbsenceRecord.Where(c => c.recordDate == d && c.recordDate == recordDate1 && c.academicPeriod == academicRecord1 && c.student.sectionName==s.sectionName).ToList();
        
            return View(av);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult addAttendance(AbsenceRecordViewMoel arvm, string submit, string delete,string selectedStudents,string gsId)
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


                if (selectedStudents != null && selectedStudents !="*")
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
                abs = db.AbsenceRecord.Where(ax=>ax.recordId==gsID1).FirstOrDefault();
                if(!(Object.ReferenceEquals(abs, null))) { 
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
    }
}
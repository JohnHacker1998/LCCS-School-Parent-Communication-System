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
            av.studentList = new List<SelectListItem>();
            //finding the student list of where the teacher is homeroom for
            av.studentList = htm.getList(currentUserId);
            //counting number of students in the retrieved list
            int a = av.studentList.Count();
            //creating an array with the size of the list of the students list to hold the selection on listbox
            av.selectedStudents = new int[a];
            Section s = new Section();
            //finding the section where the teacher is homeroom
            s = db.Section.Where(x => x.teacherId == currentUserId).FirstOrDefault();
            List<AbsenceRecord> absenceList = new List<AbsenceRecord>();
            //recording the current date as a short date string
            DateTime d = DateTime.Now.Date;
            absenceList = db.AbsenceRecord.Where(c => c.recordDate == d).ToList();
            int i = 0;
            ay = db.AcademicYear.Where(r => r.academicYearName == s.academicYearId).FirstOrDefault();
            string status = htm.whichQuarter(s.academicYearId);
            if (absenceList.Count() != 0) { 
            foreach (var k in absenceList)
            {
                av.selectedStudents[i] = k.studentId;
                i++;
            }
            }
            List<AbsenceRecord> tempRecord1 = new List<AbsenceRecord>();
            ViewBag.Message = false;
            string academicRecord1 = s.academicYearId + "-" + status;
            DateTime recordDate1 = DateTime.Now.Date;
            tempRecord1 = db.AbsenceRecord.Where(y => y.academicPeriod == academicRecord1 && y.recordDate == recordDate1).ToList();
            if (tempRecord1.Count != 0)
            {
                ViewBag.Message = true;
            }
            av.absenceList = db.AbsenceRecord.Where(rd => rd.recordDate == recordDate1 && rd.academicPeriod == academicRecord1).ToList();

            return View(av);
        }
        [HttpPost]
        public ActionResult addAttendance(AbsenceRecordViewMoel arvm, string add, string update)
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
            tempRecord1 = db.AbsenceRecord.Where(y => y.academicPeriod == academicRecord1 && y.recordDate == recordDate1).ToList();
            if (tempRecord1.Count == 0 && add!=null)
            {


                    if (arvm.selectedStudents != null)
                    {
                        List<SelectListItem> selectedItems = arvm.studentList.Where(b => arvm.selectedStudents.Contains(int.Parse(b.Value))).ToList();
                        int x = 1;
                        foreach (var j in selectedItems)
                        {

                            if (status == "Q1" || status == "Q2" || status == "Q3" || status == "Q4")
                            {

                                ar.academicPeriod = s.academicYearId + "-" + status;
                                ar.recordDate = DateTime.Now.Date;
                                ar.studentId = int.Parse(j.Value);
                                int count = htm.manageCount(ar.academicPeriod, int.Parse(j.Value));
                                ar.count = count + 1;
                                int v = 0;
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
            else if (tempRecord1 != null)
            {
                ViewBag.Message = true;
            }
                        
            if (update != null)
            {
               
                    string academicRecord = s.academicYearId + "-" + status;
                DateTime recordDate = DateTime.Now.Date;
                    tempRecord = db.AbsenceRecord.Where(y => y.academicPeriod == academicRecord && y.recordDate == recordDate).ToList();
                if (tempRecord != null || arvm.selectedStudents == null)
                {

                    //db.AbsenceRecord.Remove(tempRecord);
                    for (int v = 0; v < tempRecord.Count; v++)
                    {
                        db.AbsenceRecord.Remove(tempRecord[v]);
                        db.SaveChanges();
                    }
                     if (arvm.selectedStudents != null) { 

                    List<SelectListItem> selectedItems = new List<SelectListItem>();
                    selectedItems = arvm.studentList.Where(b => arvm.selectedStudents.Contains(int.Parse(b.Value))).ToList();
                    foreach (var j in selectedItems)
                    {
                        if (status == "Q1" || status == "Q2" || status == "Q3" || status == "Q4")
                        {

                            ar.academicPeriod = s.academicYearId + "-" + status;
                                ar.recordDate = DateTime.Now.Date;
                            ar.studentId = int.Parse(j.Value);
                            int count = htm.manageCount(ar.academicPeriod, int.Parse(j.Value));
                            ar.count = count + 1;
                            int v = 0;
                            db.AbsenceRecord.Add(ar);
                            db.SaveChanges();
                                ViewBag.successfulMessage = "Attendance update successful.";

                        }
                            else
                            {
                                if (status == "no")
                                {
                                    ViewBag.messageStatus = "Today doesn't exist in the academic year";
                                }
                                else if (status == "gap")
                                {
                                    ViewBag.messageStatus = "Today is a gap";
                                }
                            }
                        }
                }
                    }
                
               
            }
            arvm.studentList = new List<SelectListItem>();
            arvm.studentList = htm.getList(currentUserId);


            int a = arvm.studentList.Count();
            arvm.selectedStudents = new int[a];
            List<AbsenceRecord> absenceList = new List<AbsenceRecord>();
            DateTime d = DateTime.Now.Date;
            absenceList = db.AbsenceRecord.Where(c => c.recordDate == d).ToList();
            int i = 0;
            foreach (var k in absenceList)
            {
                arvm.selectedStudents[i] = k.studentId;
                i++;
            }
            arvm.absenceList = db.AbsenceRecord.Where(ax => ax.recordDate == d && ax.academicPeriod == academicRecord1).ToList();
            return View(arvm);


        }
    }
}
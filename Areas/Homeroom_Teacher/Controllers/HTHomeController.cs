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
    [Authorize(Roles = "HomeRoom")]

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
            string d = DateTime.Now.ToShortDateString();
            absenceList = db.AbsenceRecord.Where(c => c.recordDate == d).ToList();
            int i = 0;
            ay = db.AcademicYear.Where(r => r.academicYearName == s.academicYearId).FirstOrDefault();
            string status = htm.whichQuarter(s.academicYearId);
            foreach (var k in absenceList)
            {
                av.selectedStudents[i] = k.studentId;
                i++;
            }
            List<AbsenceRecord> tempRecord1 = new List<AbsenceRecord>();
            ViewBag.Message = false;
            string academicRecord1 = s.academicYearId + "-" + status;
            string recordDate1 = DateTime.Now.ToShortDateString();
            tempRecord1 = db.AbsenceRecord.Where(y => y.academicPeriod == academicRecord1 && y.recordDate == recordDate1).ToList();
            if (tempRecord1.Count != 0)
            {
                ViewBag.Message = true;
            }


            return View(av);
        }
        [HttpPost]
        public ActionResult addAttendance(AbsenceRecordViewMoel arvm, string add, string update)
        {
            HomeroomTeacherMethod htm = new HomeroomTeacherMethod();
            AbsenceRecord ar = new AbsenceRecord();
            ApplicationDbContext db = new ApplicationDbContext();
            Suspension suspension = new Suspension();
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
            string recordDate1 = DateTime.Now.ToShortDateString();
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
                                ar.recordDate = DateTime.Now.ToShortDateString();
                                ar.studentId = int.Parse(j.Value);
                                int count = htm.manageCount(ar.academicPeriod, int.Parse(j.Value));
                                ar.count = count + 1;
                                int v = 0;
                                db.AbsenceRecord.Add(ar);
                                db.SaveChanges();


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
                if (arvm.selectedStudents != null)
                {
                    string academicRecord = s.academicYearId + "-" + status;
                    string recordDate = DateTime.Now.ToShortDateString();
                    tempRecord = db.AbsenceRecord.Where(y => y.academicPeriod == academicRecord && y.recordDate == recordDate).ToList();
                    if (tempRecord != null)
                    {

                        //db.AbsenceRecord.Remove(tempRecord);
                        for (int v = 0; v < tempRecord.Count; v++)
                        {
                            db.AbsenceRecord.Remove(tempRecord[v]);
                        }

                        List<SelectListItem> selectedItems = arvm.studentList.Where(b => arvm.selectedStudents.Contains(int.Parse(b.Value))).ToList();
                        foreach (var j in selectedItems)
                        {
                            if (status == "Q1" || status == "Q2" || status == "Q3" || status == "Q4")
                            {

                                ar.academicPeriod = s.academicYearId + "-" + status;
                                ar.recordDate = DateTime.Now.ToShortDateString();
                                ar.studentId = int.Parse(j.Value);
                                int count = htm.manageCount(ar.academicPeriod, int.Parse(j.Value));
                                ar.count = count + 1;
                                int v = 0;
                                db.AbsenceRecord.Add(ar);
                                db.SaveChanges();

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
            string d = DateTime.Now.ToShortDateString();
            absenceList = db.AbsenceRecord.Where(c => c.recordDate == d).ToList();
            int i = 0;
            foreach (var k in absenceList)
            {
                arvm.selectedStudents[i] = k.studentId;
                i++;
            }

            return View(arvm);


        }
    }
}
using LCCS_School_Parent_Communication_System.Identity;
using LCCS_School_Parent_Communication_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LCCS_School_Parent_Communication_System.Additional_Class
{
    public class HomeroomTeacherMethod
    {
        public List<SelectListItem> getList(string currentUserId)
        {

            ApplicationDbContext db = new ApplicationDbContext();
            List<SelectListItem> items = new List<SelectListItem>();
            List<Student> studentLS = new List<Student>();

            studentLS = checkSuspension(currentUserId);

            if (studentLS.Count()!=0)
            {
               
                if (studentLS != null)
                {
                    foreach (var k in studentLS)
                    {
                        if (DateTime.Today.DayOfWeek != DayOfWeek.Saturday && DateTime.Today.DayOfWeek != DayOfWeek.Sunday) { 

                        items.Add(new SelectListItem { Text = k.fullName, Value = k.studentId.ToString() });
                        }
                    }
                    return items;
                }
            }

            return items;
        }
        public List<Student> checkSuspension(string currentUserId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Section s = new Section();
            Suspension checkSuspended = new Suspension();

            s = db.Section.Where(a => a.teacherId == currentUserId).FirstOrDefault();
            List<Student> studentLS = new List<Student>();
            if (s != null) {
                studentLS = db.Student.Where(a => a.sectionName == s.sectionName).ToList();
                foreach(var k in studentLS)
                {
                    checkSuspended = db.Suspension.Where(a => a.studentId == k.studentId).FirstOrDefault();
                    if (checkSuspended != null)
                    {
                        if (DateTime.Compare(DateTime.Now.Date, checkSuspended.endDate.Date)<=0)
                        {
                            studentLS.Remove(k);
                        }
                        else
                        {
                            db.Suspension.Remove(checkSuspended);
                            db.SaveChanges();
                        }
                        
                    }

                }
                return studentLS;
            }
            return studentLS;
        }
        public string whichQuarter(string academicYearName)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            AcademicYear ayr = new AcademicYear();
            string status = "";
            ayr = db.AcademicYear.Where(a => a.academicYearName == academicYearName).FirstOrDefault();
            if (ayr != null)
            {
                if (DateTime.Compare(DateTime.Now.Date, ayr.academicDurationStart) >= 0 && (DateTime.Compare(DateTime.Now.Date, ayr.academicDurationEnd) <= 0))
                {
                   
                    if (DateTime.Compare(DateTime.Now.Date,ayr.quarterOneStart) >= 0 && (DateTime.Compare(DateTime.Now.Date,ayr.quarterOneEnd) <= 0))
                    {
                        status = "Q1";
                        return status;
                    }
                    else
                    {
                        
                        if (DateTime.Compare(DateTime.Now.Date, ayr.quarterTwoStart) >= 0 && (DateTime.Compare(DateTime.Now.Date, ayr.quarterTwoEnd) <= 0))
                        {
                            status = "Q2";
                            return status;
                        }
                        else
                        {
                           
                            if (DateTime.Compare(DateTime.Now.Date,ayr.quarterThreeStart) >= 0 && (DateTime.Compare(DateTime.Now.Date, ayr.quarterThreeEnd) <= 0))
                            {
                                status = "Q3";
                                return status;
                            }
                            else 
                            {
                                                             
                                
                                if (DateTime.Compare(DateTime.Now.Date, ayr.quarterFourStart) >= 0 && (DateTime.Compare(DateTime.Now.Date, ayr.quarterFourEnd) <= 0))
                                {
                                    status = "Q4";
                                    return status;
                                }
                                else {
                                    status = "gap";
                                    return status;
                                }
                                
                            }

                        }
                    }
                }
                else
                {
                    status = "no";
                    return status;
                }

            }
            return status;
        }
        public int manageCount(string academicPeriod, int studentId)
        {

            ApplicationDbContext db = new ApplicationDbContext();
            List<AbsenceRecord> lastRecord = new List<AbsenceRecord>();

            lastRecord = db.AbsenceRecord.Where(a => a.studentId == studentId && a.academicPeriod == academicPeriod).ToList();
            int last = 0;
            if (lastRecord.Count != 0)
            {
                int a = lastRecord.Count();
                last = lastRecord[a - 1].count;
            }
            return last;
        }
    }
}
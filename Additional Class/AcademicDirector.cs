using LCCS_School_Parent_Communication_System.Identity;
using LCCS_School_Parent_Communication_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LCCS_School_Parent_Communication_System.viewModels;
using Microsoft.AspNet.Identity;

namespace LCCS_School_Parent_Communication_System.Additional_Class
{
    public class AcademicDirector
    {

        //function to record teacher information
        public void registerTeacher(Teacher teacher)
        {
            //record teacher information
            ApplicationDbContext context = new ApplicationDbContext();
            context.Teacher.Add(teacher);
            context.SaveChanges();
        }

        //function to update user information
        public void UpdateTeacher(Teacher teacher)
        {
            //update teacher with the new information
            ApplicationDbContext context = new ApplicationDbContext();
            Teacher teacherUP = new Teacher(1);

            //find the target user ising teacher id
            teacherUP = context.Teacher.Find(teacher.teacherId);

            //update teacher information
            teacherUP.subject = teacher.subject;
            teacherUP.user.fullName = teacher.user.fullName;
            teacherUP.grade = teacher.grade;
            context.SaveChanges();
        }

        //function to delete teacher information
        public void DeleteTeacher(string id)
        {
            //delete teacher record using teacher id
            ApplicationDbContext context = new ApplicationDbContext();
            Teacher teacher = new Teacher();

            //delete teacher record
            teacher = context.Teacher.Find(id);
            context.Teacher.Remove(teacher);
            context.SaveChanges();
        }

        public Boolean validateDuration(AcademicYearViewModel ayvm)
        {
            DateTime yearStartOne = Convert.ToDateTime(ayvm.yearStart);
            DateTime yearEndOne = Convert.ToDateTime(ayvm.yearEnd);
            DateTime zquarterOneStart = Convert.ToDateTime(ayvm.quarterOneStart);
            DateTime zquarterOneEnd = Convert.ToDateTime(ayvm.quarterOneEnd);
            DateTime zquarterTwoStart = Convert.ToDateTime(ayvm.quarterTwoStart);
            DateTime zquarterTwoEnd = Convert.ToDateTime(ayvm.quarterTwoEnd);
            DateTime zquarterThreeStart = Convert.ToDateTime(ayvm.quarterThreeStart);
            DateTime zquarterThreeEnd = Convert.ToDateTime(ayvm.quarterThreeEnd);
            DateTime zquarterFourStart = Convert.ToDateTime(ayvm.quarterFourStart);
            DateTime zquarterFourEnd = Convert.ToDateTime(ayvm.quarterFourEnd);

            if (DateTime.Compare(yearStartOne, yearEndOne) != 0 && DateTime.Compare(zquarterOneStart, zquarterOneEnd) != 0 &&
                DateTime.Compare(zquarterTwoStart, zquarterTwoEnd) != 0 && DateTime.Compare(zquarterThreeStart, zquarterThreeEnd) != 0 &&
                DateTime.Compare(zquarterFourStart, zquarterFourEnd) != 0)
            {

                if (DateTime.Compare(zquarterOneStart, zquarterOneEnd) < 0 && DateTime.Compare(zquarterTwoStart, zquarterTwoEnd) < 0 &&
                    DateTime.Compare(zquarterThreeStart, zquarterThreeEnd) < 0 && DateTime.Compare(zquarterFourStart, zquarterFourEnd) < 0 &&
                    DateTime.Compare(yearStartOne, yearEndOne) < 0)
                {

                    if (DateTime.Compare(zquarterOneEnd, zquarterTwoStart) < 0 && DateTime.Compare(zquarterTwoEnd, zquarterThreeStart) < 0 &&
                    DateTime.Compare(zquarterThreeEnd, zquarterFourStart) < 0 && (DateTime.Compare(zquarterFourEnd, yearEndOne) < 0 || DateTime.Compare(zquarterFourEnd, yearEndOne) == 0))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        //function to populate form data(alphabet leters, teacher and academic year)
        public SectionViewModel populateFormData()
        {
            //basic objects
            SectionViewModel sectionViewModel = new SectionViewModel();
            AcademicYearViewModel academicYearViewModel = new AcademicYearViewModel();
            ApplicationDbContext context = new ApplicationDbContext();
            ApplicationUserStore userStore = new ApplicationUserStore(context);
            ApplicationUserManager userManager = new ApplicationUserManager(userStore);
            sectionViewModel.teachers = new List<string>();
            sectionViewModel.academicYears = new List<string>();
            sectionViewModel.letter = new List<char>();
            Teacher teacher = new Teacher();
            AcademicYear academicYear = new AcademicYear();

            //retrive all teachers that have teacher role from teacher table and populate it to sectionViewModel
            var allTeachers = context.Teacher.ToList();

            if(allTeachers.Count!= 0)
            {
                foreach (var getNames in allTeachers)
                {
                    //check role
                    if (userManager.IsInRole(getNames.teacherId, "Teacher"))
                    {
                        sectionViewModel.teachers.Add(getNames.user.fullName);
                    }
                }
            }
            
            //retrive all Academic Years that are already active and populate it to sectionViewModel
            var allAcadamicYears = context.AcademicYear.ToList();

            if (allAcadamicYears.Count != 0)
            {
                foreach (var getAcadamicYear in allAcadamicYears)
                {
                    //check today is in between start and end date of the specific academic year
                    string[] duration = getAcadamicYear.duration.Split('-');
                    if (!(DateTime.Compare(DateTime.Now, DateTime.Parse(duration[0])) < 0 || DateTime.Compare(DateTime.Now, DateTime.Parse(duration[1])) > 0))
                    {
                        sectionViewModel.academicYears.Add(getAcadamicYear.academicYearName);
                    }
                }
            }
            
            //populate section letters with alphabet letters
            for (char c = 'A'; c <= 'Z'; c++)
            {
                sectionViewModel.letter.Add(c);
            }

            return sectionViewModel;
        }

        //function to populate section record
        public SectionViewModel searchSection(int grade,string letter)
        {
            //basic objects
            ApplicationDbContext context = new ApplicationDbContext();
            var sectionViewModel = new SectionViewModel();
            sectionViewModel.teachers = new List<string>();
            sectionViewModel.academicYears = new List<string>();

            //get all academic Years and check for section record existance
            var allAcadamicYears = context.AcademicYear.ToList();

            if (allAcadamicYears.Count != 0)
            {
                foreach (var getAcadamicYear in allAcadamicYears)
                {
                    //get start and end dates of a given academic year to check if today is in the middle
                    string[] duration = getAcadamicYear.duration.Split('-');
                    if (!(DateTime.Compare(DateTime.Now, DateTime.Parse(duration[0])) < 0 || DateTime.Compare(DateTime.Now, DateTime.Parse(duration[1])) > 0))
                    {
                        //search for section in a given academic year
                        var sectionRecord = context.Section.Where(s => s.sectionName == grade.ToString() + letter && s.academicYearId == getAcadamicYear.academicYearName).FirstOrDefault();

                        //check if it exists
                        if (sectionRecord != null)
                        {
                            //populate the list and return
                            sectionViewModel.teachers.Add(sectionRecord.teacher.user.fullName);
                            sectionViewModel.academicYears.Add(sectionRecord.academicYearId);
                            return sectionViewModel;
                        }
                    }
                }
            }
            
            return null;
        }

        //function to populate section list in the UI
        public List<string> populateSection()
        {
            //basic objects
            ApplicationDbContext context = new ApplicationDbContext();
            List<string> section = new List<string>();

            //get all academic years
            var academicYears = context.AcademicYear.ToList();
            foreach (var getActive in academicYears)
            {
                //get start and end dates to check if today is in the middle
                string[] duration = getActive.duration.Split('-');
                if (!(DateTime.Compare(DateTime.Now, DateTime.Parse(duration[0])) < 0 || DateTime.Compare(DateTime.Now, DateTime.Parse(duration[1])) > 0))
                {
                    //get sections on a given academic year
                    var getSection = context.Section.Where(s => s.academicYearId == getActive.academicYearName).ToList();

                    //check if sections exist in the academic year
                    if (getSection.Count != 0)
                    {
                        foreach (var getSectionName in getSection)
                        {
                            //populate section list
                            section.Add(getSectionName.sectionName);
                            
                        }
                    }
                }
            }

            return section;
        }





        public RegistrarManagementViewModel listRegistrar()
        {
            //function for listing every user with registrar role 
            var appDbContext = new ApplicationDbContext();
            var userStore = new ApplicationUserStore(appDbContext);
            var userManager = new ApplicationUserManager(userStore);
            ApplicationUser user = new ApplicationUser();
            RegistrarManagementViewModel rvm = new RegistrarManagementViewModel();

            List<ApplicationUser> users = new List<ApplicationUser>();
            users = appDbContext.Users.ToList();

            foreach (var k in users)
            {
                if (userManager.IsInRole(k.Id, "Registrar"))
                {
                    rvm.registrarList.Add(new ApplicationUser
                    {
                        Id = k.Id,
                        fullName = k.fullName,
                        UserName = k.UserName,
                        Email = k.Email

                    });
                }

            }


            return rvm;
        }
        public bool IsTeacherorUnitLeader(string uid, string role)
        {
            //function for listing every user with registrar role 
            var appDbContext = new ApplicationDbContext();
            var userStore = new ApplicationUserStore(appDbContext);
            var userManager = new ApplicationUserManager(userStore);
            ApplicationUser user = new ApplicationUser();
            RegisterTeacherViewModel rvm = new RegisterTeacherViewModel();

            List<ApplicationUser> users = new List<ApplicationUser>();
            users = appDbContext.Users.ToList();
            if (role == "Teacher") {

                if (userManager.IsInRole(uid, "Teacher"))
                {
                    return true;
                }


            }
            else if (role == "UnitLeader")
            {

                if (userManager.IsInRole(uid, "UnitLeader"))
                {
                    return true;
                }

            }
            return false;
        }

        public AcademicYearViewModel listAcademicYear()
        {
           
            ApplicationDbContext db = new ApplicationDbContext();
            AcademicYearViewModel ayVM = new AcademicYearViewModel();
            ayVM.academicList = new List<AcademicYear>();
            ayVM.academicList = db.AcademicYear.ToList();
            return ayVM;
        }
    }
}
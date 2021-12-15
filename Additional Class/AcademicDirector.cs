using LCCS_School_Parent_Communication_System.Identity;
using LCCS_School_Parent_Communication_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LCCS_School_Parent_Communication_System.Additional_Class
{
    public class AcademicDirector
    {
        public void registerTeacher(Teacher teacher)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            context.Teacher.Add(teacher);
            context.SaveChanges();
        }

        public void UpdateTeacher(Teacher teacher)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var teacherUP = context.Teacher.Find(teacher.teacherId);
            teacherUP.user.fullName = teacher.user.fullName;
            teacherUP.subject = teacher.subject;
            teacherUP.grade = teacher.grade;
            context.SaveChanges();
        }
        public void DeleteTeacher(string id)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            Teacher teacher = new Teacher();
            context.Teacher.Remove(teacher);
        }
    }
}
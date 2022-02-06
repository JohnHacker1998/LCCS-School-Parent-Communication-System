using LCCS_School_Parent_Communication_System.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace LCCS_School_Parent_Communication_System.Identity
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("scpsconn")
        {

        }

        public DbSet<Teacher> Teacher { get; set; }
        public DbSet<AcademicYear> AcademicYear { get; set; }
        public DbSet<Section> Section { get; set; }
        public DbSet<Parent> Parent { get; set; }
        public DbSet<Student> Student { get; set; }
        public DbSet<Suspension> Suspension { get; set; }
        public DbSet<LateComer> LateComer { get; set; }
        public DbSet<Warning> Warning { get; set; }

        public DbSet<AbsenceRecord> AbsenceRecord { get; set; }
        public DbSet<Evidence> Evidence { get; set; }
        public DbSet<Assignment> Assignment { get; set; }
        public DbSet<GroupStructure> GroupStructure { get; set; }
        public DbSet<GroupAssignment> GroupAssignment { get; set; }
        public DbSet<Group> Group { get; set; }
        public DbSet<StudentGroupList> StudentGroupList { get; set; }
        public DbSet<GroupStructureAssignment> GroupStructureAssignment { get; set; }

        public DbSet<Schedule> Schedule { get; set; }
        public DbSet<Result> Result { get; set; }




    }
}
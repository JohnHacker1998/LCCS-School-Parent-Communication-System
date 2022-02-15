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
        public DbSet<Announcement> Announcement { get; set; }
        public DbSet<studentAnnouncement> studentAnnouncement { get; set; }
        public DbSet<gradeAnnouncement> gradeAnnouncements { get; set; }
        public DbSet<sectionAnnouncement> sectionAnnouncement { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Parent>()
                   .HasRequired(j => j.student)
                   .WithMany()
                   .HasForeignKey(j=>j.studentId)
                   .WillCascadeOnDelete(true);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Suspension>()
                   .HasRequired(j => j.student)
                   .WithMany()
                   .HasForeignKey(j => j.studentId)
                   .WillCascadeOnDelete(true);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LateComer>()
                   .HasRequired(j => j.student)
                   .WithMany()
                   .HasForeignKey(j => j.studentId)
                   .WillCascadeOnDelete(true);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Warning>()
                   .HasRequired(j => j.student)
                   .WithMany()
                   .HasForeignKey(j => j.studentId)
                   .WillCascadeOnDelete(true);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AbsenceRecord>()
                   .HasRequired(j => j.student)
                   .WithMany()
                   .HasForeignKey(j => j.studentId)
                   .WillCascadeOnDelete(true);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StudentGroupList>()
                   .HasRequired(j => j.student)
                   .WithMany()
                   .HasForeignKey(j => j.studentId)
                   .WillCascadeOnDelete(true);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Result>()
                   .HasRequired(j => j.student)
                   .WithMany()
                   .HasForeignKey(j => j.studentId)
                   .WillCascadeOnDelete(true);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Evidence>()
                   .HasRequired(j => j.parent)
                   .WithMany()
                   .HasForeignKey(j => j.parentId)
                   .WillCascadeOnDelete(true);
            base.OnModelCreating(modelBuilder);
        }


    }
}
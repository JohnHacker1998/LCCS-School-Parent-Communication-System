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
    }
}
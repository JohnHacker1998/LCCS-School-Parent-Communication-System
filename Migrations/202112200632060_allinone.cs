namespace LCCS_School_Parent_Communication_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class allinone : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Parents",
                c => new
                    {
                        parentId = c.String(nullable: false, maxLength: 128),
                        studentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.parentId)
                .ForeignKey("dbo.Students", t => t.studentId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.parentId)
                .Index(t => t.parentId)
                .Index(t => t.studentId);
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        studentId = c.Int(nullable: false, identity: true),
                        fullName = c.String(),
                        sectionName = c.String(),
                        academicYear = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.studentId)
                .ForeignKey("dbo.AcademicYears", t => t.academicYear)
                .Index(t => t.academicYear);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Parents", "parentId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Parents", "studentId", "dbo.Students");
            DropForeignKey("dbo.Students", "academicYear", "dbo.AcademicYears");
            DropIndex("dbo.Students", new[] { "academicYear" });
            DropIndex("dbo.Parents", new[] { "studentId" });
            DropIndex("dbo.Parents", new[] { "parentId" });
            DropTable("dbo.Students");
            DropTable("dbo.Parents");
        }
    }
}

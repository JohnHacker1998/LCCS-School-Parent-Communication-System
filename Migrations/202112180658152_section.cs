namespace LCCS_School_Parent_Communication_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class section : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AcademicYears",
                c => new
                    {
                        academicYearName = c.String(nullable: false, maxLength: 128),
                        quarterOne = c.String(),
                        quarterTwo = c.String(),
                        quarterThree = c.String(),
                        quarterFour = c.String(),
                        duration = c.String(),
                    })
                .PrimaryKey(t => t.academicYearName);
            
            CreateTable(
                "dbo.Sections",
                c => new
                    {
                        sectionId = c.Int(nullable: false, identity: true),
                        academicYearId = c.String(maxLength: 128),
                        teacherId = c.String(maxLength: 128),
                        sectionName = c.String(),
                    })
                .PrimaryKey(t => t.sectionId)
                .ForeignKey("dbo.AcademicYears", t => t.academicYearId)
                .ForeignKey("dbo.Teachers", t => t.teacherId)
                .Index(t => t.academicYearId)
                .Index(t => t.teacherId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sections", "teacherId", "dbo.Teachers");
            DropForeignKey("dbo.Sections", "academicYearId", "dbo.AcademicYears");
            DropIndex("dbo.Sections", new[] { "teacherId" });
            DropIndex("dbo.Sections", new[] { "academicYearId" });
            DropTable("dbo.Sections");
            DropTable("dbo.AcademicYears");
        }
    }
}

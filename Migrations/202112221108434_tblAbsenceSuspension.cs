namespace LCCS_School_Parent_Communication_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tblAbsenceSuspension : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AbsenceRecords",
                c => new
                    {
                        recordId = c.Int(nullable: false, identity: true),
                        academicPeriod = c.String(),
                        recordDate = c.DateTime(nullable: false),
                        studentId = c.Int(nullable: false),
                        count = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.recordId)
                .ForeignKey("dbo.Students", t => t.studentId, cascadeDelete: true)
                .Index(t => t.studentId);
            
            CreateTable(
                "dbo.Suspensions",
                c => new
                    {
                        suspensionID = c.Int(nullable: false, identity: true),
                        studentId = c.Int(nullable: false),
                        startDate = c.DateTime(nullable: false),
                        endDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.suspensionID)
                .ForeignKey("dbo.Students", t => t.studentId, cascadeDelete: true)
                .Index(t => t.studentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Suspensions", "studentId", "dbo.Students");
            DropForeignKey("dbo.AbsenceRecords", "studentId", "dbo.Students");
            DropIndex("dbo.Suspensions", new[] { "studentId" });
            DropIndex("dbo.AbsenceRecords", new[] { "studentId" });
            DropTable("dbo.Suspensions");
            DropTable("dbo.AbsenceRecords");
        }
    }
}

namespace LCCS_School_Parent_Communication_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class allInOne : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LateComers",
                c => new
                    {
                        lateId = c.Int(nullable: false, identity: true),
                        studentId = c.Int(nullable: false),
                        lateDate = c.DateTime(nullable: false),
                        academicPeriod = c.String(),
                        dayCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.lateId)
                .ForeignKey("dbo.Students", t => t.studentId, cascadeDelete: true)
                .Index(t => t.studentId);
            
            CreateTable(
                "dbo.Warnings",
                c => new
                    {
                        warningId = c.Int(nullable: false, identity: true),
                        studentId = c.Int(nullable: false),
                        grade = c.Int(nullable: false),
                        warningType = c.String(),
                        WarningReadStatus = c.String(),
                    })
                .PrimaryKey(t => t.warningId)
                .ForeignKey("dbo.Students", t => t.studentId, cascadeDelete: true)
                .Index(t => t.studentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Warnings", "studentId", "dbo.Students");
            DropForeignKey("dbo.LateComers", "studentId", "dbo.Students");
            DropIndex("dbo.Warnings", new[] { "studentId" });
            DropIndex("dbo.LateComers", new[] { "studentId" });
            DropTable("dbo.Warnings");
            DropTable("dbo.LateComers");
        }
    }
}

namespace LCCS_School_Parent_Communication_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class allinone : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Results",
                c => new
                    {
                        resultId = c.Int(nullable: false, identity: true),
                        teacherId = c.String(maxLength: 128),
                        studentId = c.Int(nullable: false),
                        result = c.Int(nullable: false),
                        feedback = c.String(),
                        scheduleId = c.Int(),
                        assignmentId = c.Int(),
                        resultFor = c.String(),
                        percent = c.Int(nullable: false),
                        academicYear = c.String(),
                        grade = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.resultId)
                .ForeignKey("dbo.Assignments", t => t.assignmentId)
                .ForeignKey("dbo.Schedules", t => t.scheduleId)
                .ForeignKey("dbo.Students", t => t.studentId, cascadeDelete: true)
                .ForeignKey("dbo.Teachers", t => t.teacherId)
                .Index(t => t.teacherId)
                .Index(t => t.studentId)
                .Index(t => t.scheduleId)
                .Index(t => t.assignmentId);
            
            AddColumn("dbo.Assignments", "fileName", c => c.String());
            AddColumn("dbo.GroupStructures", "completeStatus", c => c.Int(nullable: false));
            AddColumn("dbo.GroupStructures", "teacherId", c => c.String(maxLength: 128));
            CreateIndex("dbo.GroupStructures", "teacherId");
            AddForeignKey("dbo.GroupStructures", "teacherId", "dbo.Teachers", "teacherId");
            DropColumn("dbo.Assignments", "numberOfMembers");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Assignments", "numberOfMembers", c => c.Int(nullable: false));
            DropForeignKey("dbo.Results", "teacherId", "dbo.Teachers");
            DropForeignKey("dbo.Results", "studentId", "dbo.Students");
            DropForeignKey("dbo.Results", "scheduleId", "dbo.Schedules");
            DropForeignKey("dbo.Results", "assignmentId", "dbo.Assignments");
            DropForeignKey("dbo.GroupStructures", "teacherId", "dbo.Teachers");
            DropIndex("dbo.Results", new[] { "assignmentId" });
            DropIndex("dbo.Results", new[] { "scheduleId" });
            DropIndex("dbo.Results", new[] { "studentId" });
            DropIndex("dbo.Results", new[] { "teacherId" });
            DropIndex("dbo.GroupStructures", new[] { "teacherId" });
            DropColumn("dbo.GroupStructures", "teacherId");
            DropColumn("dbo.GroupStructures", "completeStatus");
            DropColumn("dbo.Assignments", "fileName");
            DropTable("dbo.Results");
        }
    }
}

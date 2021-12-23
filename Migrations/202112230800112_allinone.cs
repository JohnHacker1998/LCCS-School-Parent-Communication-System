namespace LCCS_School_Parent_Communication_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class allinone : DbMigration
    {
        public override void Up()
        {
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
            DropIndex("dbo.Warnings", new[] { "studentId" });
            DropTable("dbo.Warnings");
        }
    }
}

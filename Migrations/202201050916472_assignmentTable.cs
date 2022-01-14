namespace LCCS_School_Parent_Communication_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class assignmentTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Assignments",
                c => new
                    {
                        assignmentId = c.Int(nullable: false, identity: true),
                        datePosted = c.DateTime(nullable: false),
                        yearlyQuarter = c.String(),
                        assignmentType = c.String(),
                        sectionID = c.Int(nullable: false),
                        submissionDate = c.DateTime(nullable: false),
                        assignmentDocument = c.Binary(),
                    })
                .PrimaryKey(t => t.assignmentId)
                .ForeignKey("dbo.Sections", t => t.sectionID, cascadeDelete: true)
                .Index(t => t.sectionID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Assignments", "sectionID", "dbo.Sections");
            DropIndex("dbo.Assignments", new[] { "sectionID" });
            DropTable("dbo.Assignments");
        }
    }
}

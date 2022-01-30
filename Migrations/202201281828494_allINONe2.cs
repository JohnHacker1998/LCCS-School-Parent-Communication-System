namespace LCCS_School_Parent_Communication_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class allINONe2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        groupId = c.Int(nullable: false, identity: true),
                        groupStrId = c.Int(nullable: false),
                        groupName = c.String(),
                    })
                .PrimaryKey(t => t.groupId)
                .ForeignKey("dbo.GroupStructures", t => t.groupStrId, cascadeDelete: true)
                .Index(t => t.groupStrId);
            
            CreateTable(
                "dbo.GroupStructures",
                c => new
                    {
                        groupStructureId = c.Int(nullable: false, identity: true),
                        academicQuarter = c.String(),
                        sectionId = c.Int(nullable: false),
                        groupStructureName = c.String(),
                        minNumberOfMembers = c.Int(nullable: false),
                        maxNumberOfMembers = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.groupStructureId)
                .ForeignKey("dbo.Sections", t => t.sectionId, cascadeDelete: true)
                .Index(t => t.sectionId);
            
            CreateTable(
                "dbo.GroupAssignments",
                c => new
                    {
                        groupAssignmentId = c.Int(nullable: false, identity: true),
                        assignmentId = c.Int(nullable: false),
                        grId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.groupAssignmentId)
                .ForeignKey("dbo.Assignments", t => t.assignmentId, cascadeDelete: true)
                .Index(t => t.assignmentId);
            
            CreateTable(
                "dbo.GroupStructureAssignments",
                c => new
                    {
                        groupStructureAssignmentId = c.Int(nullable: false, identity: true),
                        assignmentId = c.Int(nullable: false),
                        groupStructureId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.groupStructureAssignmentId)
                .ForeignKey("dbo.Assignments", t => t.assignmentId, cascadeDelete: true)
                .Index(t => t.assignmentId);
            
            CreateTable(
                "dbo.StudentGroupLists",
                c => new
                    {
                        studentId = c.Int(nullable: false),
                        groupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.studentId, t.groupId })
                .ForeignKey("dbo.Groups", t => t.groupId, cascadeDelete: true)
                .ForeignKey("dbo.Students", t => t.studentId, cascadeDelete: true)
                .Index(t => t.studentId)
                .Index(t => t.groupId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StudentGroupLists", "studentId", "dbo.Students");
            DropForeignKey("dbo.StudentGroupLists", "groupId", "dbo.Groups");
            DropForeignKey("dbo.GroupStructureAssignments", "assignmentId", "dbo.Assignments");
            DropForeignKey("dbo.GroupAssignments", "assignmentId", "dbo.Assignments");
            DropForeignKey("dbo.Groups", "groupStrId", "dbo.GroupStructures");
            DropForeignKey("dbo.GroupStructures", "sectionId", "dbo.Sections");
            DropIndex("dbo.StudentGroupLists", new[] { "groupId" });
            DropIndex("dbo.StudentGroupLists", new[] { "studentId" });
            DropIndex("dbo.GroupStructureAssignments", new[] { "assignmentId" });
            DropIndex("dbo.GroupAssignments", new[] { "assignmentId" });
            DropIndex("dbo.GroupStructures", new[] { "sectionId" });
            DropIndex("dbo.Groups", new[] { "groupStrId" });
            DropTable("dbo.StudentGroupLists");
            DropTable("dbo.GroupStructureAssignments");
            DropTable("dbo.GroupAssignments");
            DropTable("dbo.GroupStructures");
            DropTable("dbo.Groups");
        }
    }
}

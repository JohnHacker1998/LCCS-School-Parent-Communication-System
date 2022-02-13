namespace LCCS_School_Parent_Communication_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class allinOne : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Announcements",
                c => new
                    {
                        announcementID = c.Int(nullable: false, identity: true),
                        announcementTitle = c.String(),
                        announcementContent = c.String(),
                        announcementDocument = c.Binary(),
                        announcementType = c.String(),
                        endDate = c.DateTime(nullable: false),
                        postDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.announcementID);
            
            CreateTable(
                "dbo.gradeAnnouncements",
                c => new
                    {
                        announcementId = c.Int(nullable: false),
                        grade = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.announcementId, t.grade })
                .ForeignKey("dbo.Announcements", t => t.announcementId, cascadeDelete: true)
                .Index(t => t.announcementId);
            
            CreateTable(
                "dbo.sectionAnnouncements",
                c => new
                    {
                        announcementId = c.Int(nullable: false),
                        sectionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.announcementId, t.sectionId })
                .ForeignKey("dbo.Announcements", t => t.announcementId, cascadeDelete: true)
                .ForeignKey("dbo.Sections", t => t.sectionId, cascadeDelete: true)
                .Index(t => t.announcementId)
                .Index(t => t.sectionId);
            
            CreateTable(
                "dbo.studentAnnouncements",
                c => new
                    {
                        announcementId = c.Int(nullable: false),
                        studentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.announcementId, t.studentId })
                .ForeignKey("dbo.Announcements", t => t.announcementId, cascadeDelete: true)
                .ForeignKey("dbo.Students", t => t.studentId, cascadeDelete: true)
                .Index(t => t.announcementId)
                .Index(t => t.studentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.studentAnnouncements", "studentId", "dbo.Students");
            DropForeignKey("dbo.studentAnnouncements", "announcementId", "dbo.Announcements");
            DropForeignKey("dbo.sectionAnnouncements", "sectionId", "dbo.Sections");
            DropForeignKey("dbo.sectionAnnouncements", "announcementId", "dbo.Announcements");
            DropForeignKey("dbo.gradeAnnouncements", "announcementId", "dbo.Announcements");
            DropIndex("dbo.studentAnnouncements", new[] { "studentId" });
            DropIndex("dbo.studentAnnouncements", new[] { "announcementId" });
            DropIndex("dbo.sectionAnnouncements", new[] { "sectionId" });
            DropIndex("dbo.sectionAnnouncements", new[] { "announcementId" });
            DropIndex("dbo.gradeAnnouncements", new[] { "announcementId" });
            DropTable("dbo.studentAnnouncements");
            DropTable("dbo.sectionAnnouncements");
            DropTable("dbo.gradeAnnouncements");
            DropTable("dbo.Announcements");
        }
    }
}

namespace LCCS_School_Parent_Communication_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TeacherTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Teachers",
                c => new
                    {
                        teacherId = c.String(nullable: false, maxLength: 128),
                        grade = c.Int(nullable: false),
                        subject = c.String(),
                    })
                .PrimaryKey(t => t.teacherId)
                .ForeignKey("dbo.AspNetUsers", t => t.teacherId)
                .Index(t => t.teacherId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Teachers", "teacherId", "dbo.AspNetUsers");
            DropIndex("dbo.Teachers", new[] { "teacherId" });
            DropTable("dbo.Teachers");
        }
    }
}

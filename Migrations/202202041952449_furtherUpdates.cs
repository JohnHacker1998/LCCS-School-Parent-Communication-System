namespace LCCS_School_Parent_Communication_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class furtherUpdates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assignments", "fileName", c => c.String());
            AddColumn("dbo.GroupStructures", "teacherId", c => c.String(maxLength: 128));
            CreateIndex("dbo.GroupStructures", "teacherId");
            AddForeignKey("dbo.GroupStructures", "teacherId", "dbo.Teachers", "teacherId");
            DropColumn("dbo.Assignments", "numberOfMembers");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Assignments", "numberOfMembers", c => c.Int(nullable: false));
            DropForeignKey("dbo.GroupStructures", "teacherId", "dbo.Teachers");
            DropIndex("dbo.GroupStructures", new[] { "teacherId" });
            DropColumn("dbo.GroupStructures", "teacherId");
            DropColumn("dbo.Assignments", "fileName");
        }
    }
}

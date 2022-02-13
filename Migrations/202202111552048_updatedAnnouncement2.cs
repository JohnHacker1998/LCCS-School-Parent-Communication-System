namespace LCCS_School_Parent_Communication_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedAnnouncement2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Announcements", "updateStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Announcements", "updateStatus");
        }
    }
}

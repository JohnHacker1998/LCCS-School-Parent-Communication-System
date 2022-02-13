namespace LCCS_School_Parent_Communication_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedAnnouncement : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Announcements", "filName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Announcements", "filName");
        }
    }
}

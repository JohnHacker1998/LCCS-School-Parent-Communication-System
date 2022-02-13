namespace LCCS_School_Parent_Communication_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class all3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Announcements", "viewedStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Announcements", "viewedStatus");
        }
    }
}

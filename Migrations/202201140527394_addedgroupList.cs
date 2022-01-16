namespace LCCS_School_Parent_Communication_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedgroupList : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assignments", "groupList", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Assignments", "groupList");
        }
    }
}

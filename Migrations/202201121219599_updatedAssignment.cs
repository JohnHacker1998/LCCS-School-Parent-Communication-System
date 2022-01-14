namespace LCCS_School_Parent_Communication_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedAssignment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assignments", "numberOfMembers", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Assignments", "numberOfMembers");
        }
    }
}

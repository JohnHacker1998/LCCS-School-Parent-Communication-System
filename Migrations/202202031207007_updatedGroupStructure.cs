namespace LCCS_School_Parent_Communication_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedGroupStructure : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GroupStructures", "completeStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GroupStructures", "completeStatus");
        }
    }
}

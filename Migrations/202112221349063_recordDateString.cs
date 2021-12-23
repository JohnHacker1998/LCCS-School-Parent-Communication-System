namespace LCCS_School_Parent_Communication_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class recordDateString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AbsenceRecords", "recordDate", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AbsenceRecords", "recordDate", c => c.DateTime(nullable: false));
        }
    }
}

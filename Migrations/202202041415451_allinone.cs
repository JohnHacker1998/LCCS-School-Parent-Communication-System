namespace LCCS_School_Parent_Communication_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class allinone : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Schedules",
                c => new
                    {
                        scheduleId = c.Int(nullable: false, identity: true),
                        scheduleDate = c.DateTime(nullable: false),
                        scheduleFor = c.String(),
                        subject = c.String(),
                        grade = c.Int(nullable: false),
                        percentage = c.Int(nullable: false),
                        academicYear = c.String(),
                    })
                .PrimaryKey(t => t.scheduleId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Schedules");
        }
    }
}

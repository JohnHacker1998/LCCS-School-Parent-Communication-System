namespace LCCS_School_Parent_Communication_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class latestChanges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AcademicYears",
                c => new
                    {
                        academicYearName = c.String(nullable: false, maxLength: 128),
                        quarterOne = c.String(),
                        quarterTwo = c.String(),
                        quarterThree = c.String(),
                        quarterFour = c.String(),
                        duration = c.String(),
                    })
                .PrimaryKey(t => t.academicYearName);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AcademicYears");
        }
    }
}

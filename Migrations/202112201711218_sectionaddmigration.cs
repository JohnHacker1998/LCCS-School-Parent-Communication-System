namespace LCCS_School_Parent_Communication_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sectionaddmigration : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Students", name: "academicYear", newName: "academicYearId");
            RenameIndex(table: "dbo.Students", name: "IX_academicYear", newName: "IX_academicYearId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Students", name: "IX_academicYearId", newName: "IX_academicYear");
            RenameColumn(table: "dbo.Students", name: "academicYearId", newName: "academicYear");
        }
    }
}

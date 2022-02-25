namespace LCCS_School_Parent_Communication_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class all1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Evidences", "parentId", "dbo.Parents");
            DropIndex("dbo.Evidences", new[] { "parentId" });
            AlterColumn("dbo.Evidences", "parentId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Evidences", "parentId");
            AddForeignKey("dbo.Evidences", "parentId", "dbo.Parents", "parentId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Evidences", "parentId", "dbo.Parents");
            DropIndex("dbo.Evidences", new[] { "parentId" });
            AlterColumn("dbo.Evidences", "parentId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Evidences", "parentId");
            AddForeignKey("dbo.Evidences", "parentId", "dbo.Parents", "parentId");
        }
    }
}

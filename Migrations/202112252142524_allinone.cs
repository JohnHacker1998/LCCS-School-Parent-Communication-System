﻿namespace LCCS_School_Parent_Communication_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class allinone : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AbsenceRecords", "evidenceFlag", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AbsenceRecords", "evidenceFlag");
        }
    }
}
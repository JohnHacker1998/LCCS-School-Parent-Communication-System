﻿namespace LCCS_School_Parent_Communication_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class allinone : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AbsenceRecords",
                c => new
                    {
                        recordId = c.Int(nullable: false, identity: true),
                        academicPeriod = c.String(),
                        recordDate = c.DateTime(nullable: false),
                        studentId = c.Int(nullable: false),
                        count = c.Int(nullable: false),
                        evidenceFlag = c.String(),
                    })
                .PrimaryKey(t => t.recordId)
                .ForeignKey("dbo.Students", t => t.studentId, cascadeDelete: true)
                .Index(t => t.studentId);
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        studentId = c.Int(nullable: false, identity: true),
                        fullName = c.String(),
                        sectionName = c.String(),
                        academicYearId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.studentId)
                .ForeignKey("dbo.AcademicYears", t => t.academicYearId)
                .Index(t => t.academicYearId);
            
            CreateTable(
                "dbo.AcademicYears",
                c => new
                    {
                        academicYearName = c.String(nullable: false, maxLength: 128),
                        quarterOneStart = c.DateTime(nullable: false),
                        quarterOneEnd = c.DateTime(nullable: false),
                        quarterTwoStart = c.DateTime(nullable: false),
                        quarterTwoEnd = c.DateTime(nullable: false),
                        quarterThreeStart = c.DateTime(nullable: false),
                        quarterThreeEnd = c.DateTime(nullable: false),
                        quarterFourStart = c.DateTime(nullable: false),
                        quarterFourEnd = c.DateTime(nullable: false),
                        durationStart = c.DateTime(nullable: false),
                        durationEnd = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.academicYearName);
            
            CreateTable(
                "dbo.Assignments",
                c => new
                    {
                        assignmentId = c.Int(nullable: false, identity: true),
                        datePosted = c.DateTime(nullable: false),
                        yearlyQuarter = c.String(),
                        assignmentType = c.String(),
                        sectionID = c.Int(nullable: false),
                        numberOfMembers = c.Int(nullable: false),
                        submissionDate = c.DateTime(nullable: false),
                        assignmentDocument = c.Binary(),
                        groupList = c.String(),
                    })
                .PrimaryKey(t => t.assignmentId)
                .ForeignKey("dbo.Sections", t => t.sectionID, cascadeDelete: true)
                .Index(t => t.sectionID);
            
            CreateTable(
                "dbo.Sections",
                c => new
                    {
                        sectionId = c.Int(nullable: false, identity: true),
                        academicYearId = c.String(maxLength: 128),
                        teacherId = c.String(maxLength: 128),
                        sectionName = c.String(),
                    })
                .PrimaryKey(t => t.sectionId)
                .ForeignKey("dbo.AcademicYears", t => t.academicYearId)
                .ForeignKey("dbo.Teachers", t => t.teacherId)
                .Index(t => t.academicYearId)
                .Index(t => t.teacherId);
            
            CreateTable(
                "dbo.Teachers",
                c => new
                    {
                        teacherId = c.String(nullable: false, maxLength: 128),
                        grade = c.Int(nullable: false),
                        subject = c.String(),
                    })
                .PrimaryKey(t => t.teacherId)
                .ForeignKey("dbo.AspNetUsers", t => t.teacherId)
                .Index(t => t.teacherId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        fullName = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Evidences",
                c => new
                    {
                        evidenceId = c.Int(nullable: false, identity: true),
                        parentId = c.String(maxLength: 128),
                        dateUpload = c.DateTime(nullable: false),
                        evidenceDocument = c.Binary(),
                        approvalStatus = c.String(),
                    })
                .PrimaryKey(t => t.evidenceId)
                .ForeignKey("dbo.Parents", t => t.parentId)
                .Index(t => t.parentId);
            
            CreateTable(
                "dbo.Parents",
                c => new
                    {
                        parentId = c.String(nullable: false, maxLength: 128),
                        studentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.parentId)
                .ForeignKey("dbo.Students", t => t.studentId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.parentId)
                .Index(t => t.parentId)
                .Index(t => t.studentId);
            
            CreateTable(
                "dbo.LateComers",
                c => new
                    {
                        lateId = c.Int(nullable: false, identity: true),
                        studentId = c.Int(nullable: false),
                        lateDate = c.DateTime(nullable: false),
                        academicPeriod = c.String(),
                        dayCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.lateId)
                .ForeignKey("dbo.Students", t => t.studentId, cascadeDelete: true)
                .Index(t => t.studentId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.Suspensions",
                c => new
                    {
                        suspensionID = c.Int(nullable: false, identity: true),
                        studentId = c.Int(nullable: false),
                        startDate = c.DateTime(nullable: false),
                        endDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.suspensionID)
                .ForeignKey("dbo.Students", t => t.studentId, cascadeDelete: true)
                .Index(t => t.studentId);
            
            CreateTable(
                "dbo.Warnings",
                c => new
                    {
                        warningId = c.Int(nullable: false, identity: true),
                        warningDate = c.DateTime(nullable: false),
                        studentId = c.Int(nullable: false),
                        grade = c.Int(nullable: false),
                        warningType = c.String(),
                        academicYear = c.String(),
                        WarningReadStatus = c.String(),
                    })
                .PrimaryKey(t => t.warningId)
                .ForeignKey("dbo.Students", t => t.studentId, cascadeDelete: true)
                .Index(t => t.studentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Warnings", "studentId", "dbo.Students");
            DropForeignKey("dbo.Suspensions", "studentId", "dbo.Students");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.LateComers", "studentId", "dbo.Students");
            DropForeignKey("dbo.Evidences", "parentId", "dbo.Parents");
            DropForeignKey("dbo.Parents", "parentId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Parents", "studentId", "dbo.Students");
            DropForeignKey("dbo.Assignments", "sectionID", "dbo.Sections");
            DropForeignKey("dbo.Sections", "teacherId", "dbo.Teachers");
            DropForeignKey("dbo.Teachers", "teacherId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Sections", "academicYearId", "dbo.AcademicYears");
            DropForeignKey("dbo.AbsenceRecords", "studentId", "dbo.Students");
            DropForeignKey("dbo.Students", "academicYearId", "dbo.AcademicYears");
            DropIndex("dbo.Warnings", new[] { "studentId" });
            DropIndex("dbo.Suspensions", new[] { "studentId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.LateComers", new[] { "studentId" });
            DropIndex("dbo.Parents", new[] { "studentId" });
            DropIndex("dbo.Parents", new[] { "parentId" });
            DropIndex("dbo.Evidences", new[] { "parentId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Teachers", new[] { "teacherId" });
            DropIndex("dbo.Sections", new[] { "teacherId" });
            DropIndex("dbo.Sections", new[] { "academicYearId" });
            DropIndex("dbo.Assignments", new[] { "sectionID" });
            DropIndex("dbo.Students", new[] { "academicYearId" });
            DropIndex("dbo.AbsenceRecords", new[] { "studentId" });
            DropTable("dbo.Warnings");
            DropTable("dbo.Suspensions");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.LateComers");
            DropTable("dbo.Parents");
            DropTable("dbo.Evidences");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Teachers");
            DropTable("dbo.Sections");
            DropTable("dbo.Assignments");
            DropTable("dbo.AcademicYears");
            DropTable("dbo.Students");
            DropTable("dbo.AbsenceRecords");
        }
    }
}

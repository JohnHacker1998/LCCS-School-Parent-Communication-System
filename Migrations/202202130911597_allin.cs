namespace LCCS_School_Parent_Communication_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class allin : DbMigration
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
                "dbo.Announcements",
                c => new
                    {
                        announcementID = c.Int(nullable: false, identity: true),
                        announcementTitle = c.String(),
                        announcementContent = c.String(),
                        announcementDocument = c.Binary(),
                        announcementType = c.String(),
                        endDate = c.DateTime(nullable: false),
                        postDate = c.DateTime(nullable: false),
                        filName = c.String(),
                        updateStatus = c.Int(nullable: false),
                        viewedStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.announcementID);
            
            CreateTable(
                "dbo.Assignments",
                c => new
                    {
                        assignmentId = c.Int(nullable: false, identity: true),
                        datePosted = c.DateTime(nullable: false),
                        yearlyQuarter = c.String(),
                        assignmentType = c.String(),
                        sectionID = c.Int(nullable: false),
                        submissionDate = c.DateTime(nullable: false),
                        assignmentName = c.String(),
                        markPercentage = c.Int(nullable: false),
                        assignmentDocument = c.Binary(),
                        teacherId = c.String(maxLength: 128),
                        fileName = c.String(),
                    })
                .PrimaryKey(t => t.assignmentId)
                .ForeignKey("dbo.Sections", t => t.sectionID, cascadeDelete: true)
                .ForeignKey("dbo.Teachers", t => t.teacherId)
                .Index(t => t.sectionID)
                .Index(t => t.teacherId);
            
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
                "dbo.gradeAnnouncements",
                c => new
                    {
                        announcementId = c.Int(nullable: false),
                        grade = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.announcementId, t.grade })
                .ForeignKey("dbo.Announcements", t => t.announcementId, cascadeDelete: true)
                .Index(t => t.announcementId);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        groupId = c.Int(nullable: false, identity: true),
                        groupStrId = c.Int(nullable: false),
                        groupName = c.String(),
                    })
                .PrimaryKey(t => t.groupId)
                .ForeignKey("dbo.GroupStructures", t => t.groupStrId, cascadeDelete: true)
                .Index(t => t.groupStrId);
            
            CreateTable(
                "dbo.GroupStructures",
                c => new
                    {
                        groupStructureId = c.Int(nullable: false, identity: true),
                        academicQuarter = c.String(),
                        sectionId = c.Int(nullable: false),
                        groupStructureName = c.String(),
                        minNumberOfMembers = c.Int(nullable: false),
                        maxNumberOfMembers = c.Int(nullable: false),
                        completeStatus = c.Int(nullable: false),
                        teacherId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.groupStructureId)
                .ForeignKey("dbo.Sections", t => t.sectionId, cascadeDelete: true)
                .ForeignKey("dbo.Teachers", t => t.teacherId)
                .Index(t => t.sectionId)
                .Index(t => t.teacherId);
            
            CreateTable(
                "dbo.GroupAssignments",
                c => new
                    {
                        groupAssignmentId = c.Int(nullable: false, identity: true),
                        assignmentId = c.Int(nullable: false),
                        grId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.groupAssignmentId)
                .ForeignKey("dbo.Assignments", t => t.assignmentId, cascadeDelete: true)
                .Index(t => t.assignmentId);
            
            CreateTable(
                "dbo.GroupStructureAssignments",
                c => new
                    {
                        groupStructureAssignmentId = c.Int(nullable: false, identity: true),
                        assignmentId = c.Int(nullable: false),
                        groupStructureId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.groupStructureAssignmentId)
                .ForeignKey("dbo.Assignments", t => t.assignmentId, cascadeDelete: true)
                .Index(t => t.assignmentId);
            
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
                "dbo.Results",
                c => new
                    {
                        resultId = c.Int(nullable: false, identity: true),
                        teacherId = c.String(maxLength: 128),
                        studentId = c.Int(nullable: false),
                        result = c.Int(nullable: false),
                        feedback = c.String(),
                        scheduleId = c.Int(),
                        assignmentId = c.Int(),
                        resultFor = c.String(),
                        percent = c.Int(nullable: false),
                        academicYear = c.String(),
                        grade = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.resultId)
                .ForeignKey("dbo.Assignments", t => t.assignmentId)
                .ForeignKey("dbo.Schedules", t => t.scheduleId)
                .ForeignKey("dbo.Students", t => t.studentId, cascadeDelete: true)
                .ForeignKey("dbo.Teachers", t => t.teacherId)
                .Index(t => t.teacherId)
                .Index(t => t.studentId)
                .Index(t => t.scheduleId)
                .Index(t => t.assignmentId);
            
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
                "dbo.sectionAnnouncements",
                c => new
                    {
                        announcementId = c.Int(nullable: false),
                        sectionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.announcementId, t.sectionId })
                .ForeignKey("dbo.Announcements", t => t.announcementId, cascadeDelete: true)
                .ForeignKey("dbo.Sections", t => t.sectionId, cascadeDelete: true)
                .Index(t => t.announcementId)
                .Index(t => t.sectionId);
            
            CreateTable(
                "dbo.studentAnnouncements",
                c => new
                    {
                        announcementId = c.Int(nullable: false),
                        studentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.announcementId, t.studentId })
                .ForeignKey("dbo.Announcements", t => t.announcementId, cascadeDelete: true)
                .ForeignKey("dbo.Students", t => t.studentId, cascadeDelete: true)
                .Index(t => t.announcementId)
                .Index(t => t.studentId);
            
            CreateTable(
                "dbo.StudentGroupLists",
                c => new
                    {
                        studentId = c.Int(nullable: false),
                        groupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.studentId, t.groupId })
                .ForeignKey("dbo.Groups", t => t.groupId, cascadeDelete: true)
                .ForeignKey("dbo.Students", t => t.studentId, cascadeDelete: true)
                .Index(t => t.studentId)
                .Index(t => t.groupId);
            
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
            DropForeignKey("dbo.StudentGroupLists", "studentId", "dbo.Students");
            DropForeignKey("dbo.StudentGroupLists", "groupId", "dbo.Groups");
            DropForeignKey("dbo.studentAnnouncements", "studentId", "dbo.Students");
            DropForeignKey("dbo.studentAnnouncements", "announcementId", "dbo.Announcements");
            DropForeignKey("dbo.sectionAnnouncements", "sectionId", "dbo.Sections");
            DropForeignKey("dbo.sectionAnnouncements", "announcementId", "dbo.Announcements");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Results", "teacherId", "dbo.Teachers");
            DropForeignKey("dbo.Results", "studentId", "dbo.Students");
            DropForeignKey("dbo.Results", "scheduleId", "dbo.Schedules");
            DropForeignKey("dbo.Results", "assignmentId", "dbo.Assignments");
            DropForeignKey("dbo.LateComers", "studentId", "dbo.Students");
            DropForeignKey("dbo.GroupStructureAssignments", "assignmentId", "dbo.Assignments");
            DropForeignKey("dbo.GroupAssignments", "assignmentId", "dbo.Assignments");
            DropForeignKey("dbo.Groups", "groupStrId", "dbo.GroupStructures");
            DropForeignKey("dbo.GroupStructures", "teacherId", "dbo.Teachers");
            DropForeignKey("dbo.GroupStructures", "sectionId", "dbo.Sections");
            DropForeignKey("dbo.gradeAnnouncements", "announcementId", "dbo.Announcements");
            DropForeignKey("dbo.Evidences", "parentId", "dbo.Parents");
            DropForeignKey("dbo.Parents", "parentId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Parents", "studentId", "dbo.Students");
            DropForeignKey("dbo.Assignments", "teacherId", "dbo.Teachers");
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
            DropIndex("dbo.StudentGroupLists", new[] { "groupId" });
            DropIndex("dbo.StudentGroupLists", new[] { "studentId" });
            DropIndex("dbo.studentAnnouncements", new[] { "studentId" });
            DropIndex("dbo.studentAnnouncements", new[] { "announcementId" });
            DropIndex("dbo.sectionAnnouncements", new[] { "sectionId" });
            DropIndex("dbo.sectionAnnouncements", new[] { "announcementId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Results", new[] { "assignmentId" });
            DropIndex("dbo.Results", new[] { "scheduleId" });
            DropIndex("dbo.Results", new[] { "studentId" });
            DropIndex("dbo.Results", new[] { "teacherId" });
            DropIndex("dbo.LateComers", new[] { "studentId" });
            DropIndex("dbo.GroupStructureAssignments", new[] { "assignmentId" });
            DropIndex("dbo.GroupAssignments", new[] { "assignmentId" });
            DropIndex("dbo.GroupStructures", new[] { "teacherId" });
            DropIndex("dbo.GroupStructures", new[] { "sectionId" });
            DropIndex("dbo.Groups", new[] { "groupStrId" });
            DropIndex("dbo.gradeAnnouncements", new[] { "announcementId" });
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
            DropIndex("dbo.Assignments", new[] { "teacherId" });
            DropIndex("dbo.Assignments", new[] { "sectionID" });
            DropIndex("dbo.Students", new[] { "academicYearId" });
            DropIndex("dbo.AbsenceRecords", new[] { "studentId" });
            DropTable("dbo.Warnings");
            DropTable("dbo.Suspensions");
            DropTable("dbo.StudentGroupLists");
            DropTable("dbo.studentAnnouncements");
            DropTable("dbo.sectionAnnouncements");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Schedules");
            DropTable("dbo.Results");
            DropTable("dbo.LateComers");
            DropTable("dbo.GroupStructureAssignments");
            DropTable("dbo.GroupAssignments");
            DropTable("dbo.GroupStructures");
            DropTable("dbo.Groups");
            DropTable("dbo.gradeAnnouncements");
            DropTable("dbo.Parents");
            DropTable("dbo.Evidences");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Teachers");
            DropTable("dbo.Sections");
            DropTable("dbo.Assignments");
            DropTable("dbo.Announcements");
            DropTable("dbo.AcademicYears");
            DropTable("dbo.Students");
            DropTable("dbo.AbsenceRecords");
        }
    }
}

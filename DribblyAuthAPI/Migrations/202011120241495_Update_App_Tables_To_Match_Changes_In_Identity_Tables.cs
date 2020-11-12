namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_App_Tables_To_Match_Changes_In_Identity_Tables : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.CourtFollowings");
            AlterColumn("dbo.UserActivities", "UserId", c => c.Long(nullable: false));
            AlterColumn("dbo.Photos", "UploadedById", c => c.Long(nullable: false));
            AlterColumn("dbo.Videos", "AddedBy", c => c.Long(nullable: false));
            AlterColumn("dbo.Bookings", "AddedBy", c => c.Long(nullable: false));
            AlterColumn("dbo.Bookings", "BookedById", c => c.Long(nullable: false));
            AlterColumn("dbo.Courts", "OwnerId", c => c.Long(nullable: false));
            AlterColumn("dbo.Contacts", "AddedBy", c => c.Long(nullable: false));
            AlterColumn("dbo.CourtFollowings", "FollowedById", c => c.Long(nullable: false));
            AlterColumn("dbo.CourtReviews", "ReviewedById", c => c.Long(nullable: false));
            AlterColumn("dbo.ExceptionLogs", "LoggedBy", c => c.Long());
            AlterColumn("dbo.Games", "AddedById", c => c.Long(nullable: false));
            AlterColumn("dbo.Notifications", "ForUserId", c => c.Long(nullable: false));
            AlterColumn("dbo.Posts", "AddedById", c => c.Long(nullable: false));
            AddPrimaryKey("dbo.CourtFollowings", new[] { "CourtId", "FollowedById" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.CourtFollowings");
            AlterColumn("dbo.Posts", "AddedById", c => c.String(nullable: false));
            AlterColumn("dbo.Notifications", "ForUserId", c => c.String());
            AlterColumn("dbo.Games", "AddedById", c => c.String(nullable: false));
            AlterColumn("dbo.ExceptionLogs", "LoggedBy", c => c.String());
            AlterColumn("dbo.CourtReviews", "ReviewedById", c => c.String());
            AlterColumn("dbo.CourtFollowings", "FollowedById", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Contacts", "AddedBy", c => c.String());
            AlterColumn("dbo.Courts", "OwnerId", c => c.String());
            AlterColumn("dbo.Bookings", "BookedById", c => c.String());
            AlterColumn("dbo.Bookings", "AddedBy", c => c.String(nullable: false));
            AlterColumn("dbo.Videos", "AddedBy", c => c.String());
            AlterColumn("dbo.Photos", "UploadedById", c => c.String());
            AlterColumn("dbo.UserActivities", "UserId", c => c.String());
            AddPrimaryKey("dbo.CourtFollowings", new[] { "CourtId", "FollowedById" });
        }
    }
}

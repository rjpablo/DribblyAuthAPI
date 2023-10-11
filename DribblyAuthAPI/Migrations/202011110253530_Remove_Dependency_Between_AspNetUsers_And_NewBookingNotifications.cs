namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remove_Dependency_Between_AspNetUsers_And_NewBookingNotifications : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.NewBookingNotifications", "FK_dbo.GameBookedNotifications_dbo.AspNetUsers_BookedById");
            DropForeignKey("dbo.Accounts", "IdentityUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Accounts", new[] { "IdentityUserId" });
            DropIndex("dbo.NewBookingNotifications", new[] { "BookedById" });
        }
        
        public override void Down()
        {            
            CreateIndex("dbo.NewBookingNotifications", "BookedById");
            CreateIndex("dbo.Accounts", "IdentityUserId");
            AddForeignKey("dbo.Accounts", "IdentityUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.NewBookingNotifications", "BookedById", "dbo.AspNetUsers", "Id", name: "FK_dbo.GameBookedNotifications_dbo.AspNetUsers_BookedById");
        }
    }
}

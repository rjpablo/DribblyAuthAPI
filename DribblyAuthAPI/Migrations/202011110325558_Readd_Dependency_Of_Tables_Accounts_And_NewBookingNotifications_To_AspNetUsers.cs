namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Readd_Dependency_Of_Tables_Accounts_And_NewBookingNotifications_To_AspNetUsers : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.NewBookingNotifications", "BookedById", c => c.Long(nullable: false));
            CreateIndex("dbo.NewBookingNotifications", "BookedById");
            AddForeignKey("dbo.NewBookingNotifications", "BookedById", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            //CreateIndex("dbo.Accounts", "IdentityUserId");
            //AddForeignKey("dbo.Accounts", "IdentityUserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Accounts", "IdentityUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Accounts", new[] { "IdentityUserId" });
            DropForeignKey("dbo.NewBookingNotifications", "BookedById", "dbo.AspNetUsers");
            DropIndex("dbo.NewBookingNotifications", new[] { "BookedById" });
            AlterColumn("dbo.NewBookingNotifications", "BookedById", c => c.String());
        }
    }
}

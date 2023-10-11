namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modify_NewBookingNotifications_Rename_Game_To_Booking : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.NewBookingNotifications", "FK_dbo.NewBookingNotifications_dbo.Games_GameId");
            DropForeignKey("dbo.NewBookingNotifications", "FK_dbo.GameBookedNotifications_dbo.Games_GameId");
            DropIndex("dbo.NewBookingNotifications", new[] { "GameId" });
            AddColumn("dbo.NewBookingNotifications", "BookingId", c => c.Long(nullable: false));
            CreateIndex("dbo.NewBookingNotifications", "BookingId");
            AddForeignKey("dbo.NewBookingNotifications", "BookingId", "dbo.Bookings", "Id", cascadeDelete: true);
            DropColumn("dbo.NewBookingNotifications", "GameId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.NewBookingNotifications", "GameId", c => c.Long(nullable: false));
            DropForeignKey("dbo.NewBookingNotifications", "BookingId", "dbo.Bookings");
            DropIndex("dbo.NewBookingNotifications", new[] { "BookingId" });
            DropColumn("dbo.NewBookingNotifications", "BookingId");
            CreateIndex("dbo.NewBookingNotifications", "GameId");
            AddForeignKey("dbo.NewBookingNotifications", "GameId", "dbo.Games", "Id", name: "FK_dbo.NewBookingNotifications_dbo.Games_GameId");
            AddForeignKey("dbo.NewBookingNotifications", "GameId", "dbo.Games", "Id", name: "FK_dbo.GameBookedNotifications_dbo.Games_GameId");
        }
    }
}

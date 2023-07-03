namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropTable_NewBookingNotifications : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.NewBookingNotifications", "Id", "dbo.Notifications");
            DropForeignKey("dbo.NewBookingNotifications", "BookingId", "dbo.Bookings");
            DropForeignKey("dbo.NewBookingNotifications", "BookedById", "dbo.AspNetUsers");
            DropIndex("dbo.NewBookingNotifications", new[] { "Id" });
            DropIndex("dbo.NewBookingNotifications", new[] { "BookingId" });
            DropIndex("dbo.NewBookingNotifications", new[] { "BookedById" });
            DropTable("dbo.NewBookingNotifications");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.NewBookingNotifications",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        BookingId = c.Long(nullable: false),
                        BookedById = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.NewBookingNotifications", "BookedById");
            CreateIndex("dbo.NewBookingNotifications", "BookingId");
            CreateIndex("dbo.NewBookingNotifications", "Id");
            AddForeignKey("dbo.NewBookingNotifications", "BookedById", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.NewBookingNotifications", "BookingId", "dbo.Bookings", "Id", cascadeDelete: true);
            AddForeignKey("dbo.NewBookingNotifications", "Id", "dbo.Notifications", "Id");
        }
    }
}

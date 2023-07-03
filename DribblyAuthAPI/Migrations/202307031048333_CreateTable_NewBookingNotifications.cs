namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTable_NewBookingNotifications : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NewBookingNotifications",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        BookingId = c.Long(nullable: false),
                        BookedById = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Notifications", t => t.Id)
                .ForeignKey("dbo.Bookings", t => t.BookingId, cascadeDelete: true)
                .ForeignKey("dbo.Accounts", t => t.BookedById, cascadeDelete: true)
                .Index(t => t.Id)
                .Index(t => t.BookingId)
                .Index(t => t.BookedById);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NewBookingNotifications", "BookedById", "dbo.Accounts");
            DropForeignKey("dbo.NewBookingNotifications", "BookingId", "dbo.Bookings");
            DropForeignKey("dbo.NewBookingNotifications", "Id", "dbo.Notifications");
            DropIndex("dbo.NewBookingNotifications", new[] { "BookedById" });
            DropIndex("dbo.NewBookingNotifications", new[] { "BookingId" });
            DropIndex("dbo.NewBookingNotifications", new[] { "Id" });
            DropTable("dbo.NewBookingNotifications");
        }
    }
}

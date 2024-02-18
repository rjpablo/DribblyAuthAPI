namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameFK_Events_CourtId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Bookings", "FK_dbo.Events_dbo.Courts_CourtId");
            AddForeignKey("dbo.Bookings", "CourtId", "dbo.Courts");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bookings", "CourtId", "dbo.Courts");
            AddForeignKey("dbo.Bookings", "CourtId", "dbo.Courts", name: "FK_dbo.Events_dbo.Courts_CourtId");
        }
    }
}

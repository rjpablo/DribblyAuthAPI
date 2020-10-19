namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_Table_CourtReviews_Rename_Column_EventId_To_BookingId : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.CourtReviews", name: "EventId", newName: "BookingId");
            // The index below was not creted when column "EventId" was added, so renaming it causes an error
            // RenameIndex(table: "dbo.CourtReviews", name: "IX_EventId", newName: "IX_BookingId");
        }

        public override void Down()
        {
            // The index below was not creted when column "EventId" was added, so renaming it causes an error
            // RenameIndex(table: "dbo.CourtReviews", name: "IX_BookingId", newName: "IX_EventId");
            RenameColumn(table: "dbo.CourtReviews", name: "BookingId", newName: "EventId");
        }
    }
}

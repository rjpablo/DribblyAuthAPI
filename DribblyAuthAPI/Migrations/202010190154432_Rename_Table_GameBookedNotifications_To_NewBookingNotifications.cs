namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Rename_Table_GameBookedNotifications_To_NewBookingNotifications : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.GameBookedNotifications", newName: "NewBookingNotifications");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.NewBookingNotifications", newName: "GameBookedNotifications");
        }
    }
}

namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Rename_Table_Events_To_Bookings : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Events", newName: "Bookings");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Bookings", newName: "Events");
        }
    }
}

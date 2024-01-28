namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumns_AccountLatitude_and_AccountLongitude : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "Latitude", c => c.Double());
            AddColumn("dbo.Accounts", "Longitude", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accounts", "Longitude");
            DropColumn("dbo.Accounts", "Latitude");
        }
    }
}

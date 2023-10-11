namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CourtModel_RemoveAddressAddLatitudeLongitudeIsPublic : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courts", "Latitude", c => c.Double(nullable: false));
            AddColumn("dbo.Courts", "Longitude", c => c.Double(nullable: false));
            AddColumn("dbo.Courts", "IsPublic", c => c.Boolean(nullable: false));
            DropColumn("dbo.Courts", "Address");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Courts", "Address", c => c.String());
            DropColumn("dbo.Courts", "IsPublic");
            DropColumn("dbo.Courts", "Longitude");
            DropColumn("dbo.Courts", "Latitude");
        }
    }
}

namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Populate_AccountLatitude_and_AccountLongitude : DbMigration
    {
        public override void Up()
        {

            Sql(@"UPDATE a
                SET a.Longitude = c.Lng, a.Latitude = c.Lat
                FROM Players p
                    INNER JOIN Accounts a
                    ON p.Id = a.Id
                    INNER JOIN Cities c
                    ON p.CityId = c.Id;");
        }
        
        public override void Down()
        {
        }
    }
}

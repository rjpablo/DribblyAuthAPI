namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modify_CourtModel_MakeRatePerHourNonNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Courts", "RatePerHour", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Courts", "RatePerHour", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}

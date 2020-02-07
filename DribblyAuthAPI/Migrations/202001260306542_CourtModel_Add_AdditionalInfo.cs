namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CourtModel_Add_AdditionalInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courts", "AdditionalInfo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Courts", "AdditionalInfo");
        }
    }
}

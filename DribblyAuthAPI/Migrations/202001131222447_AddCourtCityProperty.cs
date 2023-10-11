namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCourtCityProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courts", "City", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Courts", "City");
        }
    }
}

namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class renameCourtCityToAddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courts", "Address", c => c.String());
            DropColumn("dbo.Courts", "City");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Courts", "City", c => c.String());
            DropColumn("dbo.Courts", "Address");
        }
    }
}

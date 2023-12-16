namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumn_PlayersFbLink : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "FbLink", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Players", "FbLink");
        }
    }
}

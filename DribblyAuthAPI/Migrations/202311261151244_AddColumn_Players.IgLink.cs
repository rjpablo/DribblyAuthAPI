namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumn_PlayersIgLink : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "IgLink", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Players", "IgLink");
        }
    }
}

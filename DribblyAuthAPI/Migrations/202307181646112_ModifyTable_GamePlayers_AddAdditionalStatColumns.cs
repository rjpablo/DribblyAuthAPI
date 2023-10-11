namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_GamePlayers_AddAdditionalStatColumns : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GamePlayers", "OReb", c => c.Int(nullable: false));
            AddColumn("dbo.GamePlayers", "DReb", c => c.Int(nullable: false));
            AddColumn("dbo.GamePlayers", "FGA", c => c.Int(nullable: false));
            AddColumn("dbo.GamePlayers", "FGM", c => c.Int(nullable: false));
            AddColumn("dbo.GamePlayers", "ThreePA", c => c.Int(nullable: false));
            AddColumn("dbo.GamePlayers", "ThreePM", c => c.Int(nullable: false));
            AddColumn("dbo.GamePlayers", "FTA", c => c.Int(nullable: false));
            AddColumn("dbo.GamePlayers", "FTM", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GamePlayers", "FTM");
            DropColumn("dbo.GamePlayers", "FTA");
            DropColumn("dbo.GamePlayers", "ThreePM");
            DropColumn("dbo.GamePlayers", "ThreePA");
            DropColumn("dbo.GamePlayers", "FGM");
            DropColumn("dbo.GamePlayers", "FGA");
            DropColumn("dbo.GamePlayers", "DReb");
            DropColumn("dbo.GamePlayers", "OReb");
        }
    }
}

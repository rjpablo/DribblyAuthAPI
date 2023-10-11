namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_GameTeams_AddAdditionalStatsColumns : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GameTeams", "Won", c => c.Boolean());
            AddColumn("dbo.GameTeams", "FGA", c => c.Int(nullable: false));
            AddColumn("dbo.GameTeams", "FGM", c => c.Int(nullable: false));
            AddColumn("dbo.GameTeams", "ThreePA", c => c.Int(nullable: false));
            AddColumn("dbo.GameTeams", "ThreePM", c => c.Int(nullable: false));
            AddColumn("dbo.GameTeams", "Blocks", c => c.Int(nullable: false));
            AddColumn("dbo.GameTeams", "Rebounds", c => c.Int(nullable: false));
            AddColumn("dbo.GameTeams", "Assists", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GameTeams", "Assists");
            DropColumn("dbo.GameTeams", "Rebounds");
            DropColumn("dbo.GameTeams", "Blocks");
            DropColumn("dbo.GameTeams", "ThreePM");
            DropColumn("dbo.GameTeams", "ThreePA");
            DropColumn("dbo.GameTeams", "FGM");
            DropColumn("dbo.GameTeams", "FGA");
            DropColumn("dbo.GameTeams", "Won");
        }
    }
}

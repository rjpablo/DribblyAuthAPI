namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Steal_Columns : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StageTeams", "SPG", c => c.Int(nullable: false));
            AddColumn("dbo.TournamentTeams", "SPG", c => c.Int(nullable: false));
            AddColumn("dbo.GameTeams", "Steals", c => c.Int(nullable: false));
            AddColumn("dbo.GamePlayers", "Steals", c => c.Int(nullable: false));
            AddColumn("dbo.PlayerStats", "SPG", c => c.Int(nullable: false));
            AddColumn("dbo.TeamStats", "SPG", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TeamStats", "SPG");
            DropColumn("dbo.PlayerStats", "SPG");
            DropColumn("dbo.GamePlayers", "Steals");
            DropColumn("dbo.GameTeams", "Steals");
            DropColumn("dbo.TournamentTeams", "SPG");
            DropColumn("dbo.StageTeams", "SPG");
        }
    }
}

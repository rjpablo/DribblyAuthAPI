namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Turnover_Columns : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StageTeams", "TPG", c => c.Int(nullable: false));
            AddColumn("dbo.TournamentTeams", "TPG", c => c.Int(nullable: false));
            AddColumn("dbo.GameTeams", "Turnovers", c => c.Int(nullable: false));
            AddColumn("dbo.GamePlayers", "Turnovers", c => c.Int(nullable: false));
            AddColumn("dbo.PlayerStats", "TPG", c => c.Int(nullable: false));
            AddColumn("dbo.TeamStats", "TPG", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TeamStats", "TPG");
            DropColumn("dbo.PlayerStats", "TPG");
            DropColumn("dbo.GamePlayers", "Turnovers");
            DropColumn("dbo.GameTeams", "Turnovers");
            DropColumn("dbo.TournamentTeams", "TPG");
            DropColumn("dbo.StageTeams", "TPG");
        }
    }
}

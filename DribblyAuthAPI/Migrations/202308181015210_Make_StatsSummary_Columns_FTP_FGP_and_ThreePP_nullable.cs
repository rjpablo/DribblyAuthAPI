namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Make_StatsSummary_Columns_FTP_FGP_and_ThreePP_nullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.StageTeams", "FGP", c => c.Double());
            AlterColumn("dbo.StageTeams", "ThreePP", c => c.Double());
            AlterColumn("dbo.StageTeams", "FTP", c => c.Double());
            AlterColumn("dbo.TeamMemberships", "FGP", c => c.Double());
            AlterColumn("dbo.TeamMemberships", "ThreePP", c => c.Double());
            AlterColumn("dbo.TeamMemberships", "FTP", c => c.Double());
            AlterColumn("dbo.TournamentTeams", "FGP", c => c.Double());
            AlterColumn("dbo.TournamentTeams", "ThreePP", c => c.Double());
            AlterColumn("dbo.TournamentTeams", "FTP", c => c.Double());
            AlterColumn("dbo.PlayerStats", "FGP", c => c.Double());
            AlterColumn("dbo.PlayerStats", "ThreePP", c => c.Double());
            AlterColumn("dbo.PlayerStats", "FTP", c => c.Double());
            AlterColumn("dbo.TeamStats", "FGP", c => c.Double());
            AlterColumn("dbo.TeamStats", "ThreePP", c => c.Double());
            AlterColumn("dbo.TeamStats", "FTP", c => c.Double());
            AlterColumn("dbo.TournamentPlayers", "FGP", c => c.Double());
            AlterColumn("dbo.TournamentPlayers", "ThreePP", c => c.Double());
            AlterColumn("dbo.TournamentPlayers", "FTP", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TournamentPlayers", "FTP", c => c.Double(nullable: false));
            AlterColumn("dbo.TournamentPlayers", "ThreePP", c => c.Double(nullable: false));
            AlterColumn("dbo.TournamentPlayers", "FGP", c => c.Double(nullable: false));
            AlterColumn("dbo.TeamStats", "FTP", c => c.Double(nullable: false));
            AlterColumn("dbo.TeamStats", "ThreePP", c => c.Double(nullable: false));
            AlterColumn("dbo.TeamStats", "FGP", c => c.Double(nullable: false));
            AlterColumn("dbo.PlayerStats", "FTP", c => c.Double(nullable: false));
            AlterColumn("dbo.PlayerStats", "ThreePP", c => c.Double(nullable: false));
            AlterColumn("dbo.PlayerStats", "FGP", c => c.Double(nullable: false));
            AlterColumn("dbo.TournamentTeams", "FTP", c => c.Double(nullable: false));
            AlterColumn("dbo.TournamentTeams", "ThreePP", c => c.Double(nullable: false));
            AlterColumn("dbo.TournamentTeams", "FGP", c => c.Double(nullable: false));
            AlterColumn("dbo.TeamMemberships", "FTP", c => c.Double(nullable: false));
            AlterColumn("dbo.TeamMemberships", "ThreePP", c => c.Double(nullable: false));
            AlterColumn("dbo.TeamMemberships", "FGP", c => c.Double(nullable: false));
            AlterColumn("dbo.StageTeams", "FTP", c => c.Double(nullable: false));
            AlterColumn("dbo.StageTeams", "ThreePP", c => c.Double(nullable: false));
            AlterColumn("dbo.StageTeams", "FGP", c => c.Double(nullable: false));
        }
    }
}

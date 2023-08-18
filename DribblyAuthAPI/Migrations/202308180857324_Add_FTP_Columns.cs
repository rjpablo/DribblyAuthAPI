namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_FTP_Columns : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StageTeams", "FTP", c => c.Double(nullable: false));
            AddColumn("dbo.TeamMemberships", "FTP", c => c.Double(nullable: false));
            AddColumn("dbo.TournamentTeams", "FTP", c => c.Double(nullable: false));
            AddColumn("dbo.TeamStats", "FTP", c => c.Double(nullable: false));
            AddColumn("dbo.TournamentPlayers", "FTP", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TournamentPlayers", "FTP");
            DropColumn("dbo.TeamStats", "FTP");
            DropColumn("dbo.TournamentTeams", "FTP");
            DropColumn("dbo.TeamMemberships", "FTP");
            DropColumn("dbo.StageTeams", "FTP");
        }
    }
}

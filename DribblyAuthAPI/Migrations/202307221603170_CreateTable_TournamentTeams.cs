namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTable_TournamentTeams : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TournamentTeams",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TeamId = c.Long(nullable: false),
                        TournamentId = c.Long(nullable: false),
                        GP = c.Int(nullable: false),
                        GW = c.Int(nullable: false),
                        PPG = c.Double(nullable: false),
                        RPG = c.Double(nullable: false),
                        APG = c.Double(nullable: false),
                        FGP = c.Double(nullable: false),
                        BPG = c.Double(nullable: false),
                        ThreePP = c.Double(nullable: false),
                        OverallScore = c.Double(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Teams", t => t.TeamId, cascadeDelete: true)
                .ForeignKey("dbo.Tournaments", t => t.TournamentId, cascadeDelete: true)
                .Index(t => t.TeamId)
                .Index(t => t.TournamentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TournamentTeams", "TournamentId", "dbo.Tournaments");
            DropForeignKey("dbo.TournamentTeams", "TeamId", "dbo.Teams");
            DropIndex("dbo.TournamentTeams", new[] { "TournamentId" });
            DropIndex("dbo.TournamentTeams", new[] { "TeamId" });
            DropTable("dbo.TournamentTeams");
        }
    }
}

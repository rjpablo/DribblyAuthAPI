namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTable_TournamentPlayers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TournamentPlayers",
                c => new
                    {
                        AccountId = c.Long(nullable: false),
                        TournamentId = c.Long(nullable: false),
                        GP = c.Int(nullable: false),
                        GW = c.Int(nullable: false),
                        PPG = c.Double(nullable: false),
                        RPG = c.Double(nullable: false),
                        APG = c.Double(nullable: false),
                        FGP = c.Double(nullable: false),
                        BPG = c.Double(nullable: false),
                        TPG = c.Int(nullable: false),
                        SPG = c.Int(nullable: false),
                        ThreePP = c.Double(nullable: false),
                        OverallScore = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.AccountId, t.TournamentId })
                .ForeignKey("dbo.Accounts", t => t.AccountId, cascadeDelete: true)
                .ForeignKey("dbo.Tournaments", t => t.TournamentId, cascadeDelete: true)
                .Index(t => t.AccountId)
                .Index(t => t.TournamentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TournamentPlayers", "TournamentId", "dbo.Tournaments");
            DropForeignKey("dbo.TournamentPlayers", "AccountId", "dbo.Accounts");
            DropIndex("dbo.TournamentPlayers", new[] { "TournamentId" });
            DropIndex("dbo.TournamentPlayers", new[] { "AccountId" });
            DropTable("dbo.TournamentPlayers");
        }
    }
}

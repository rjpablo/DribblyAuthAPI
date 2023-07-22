namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTable_JoinTournamentRequests : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.JoinTournamentRequests",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TournamentId = c.Long(nullable: false),
                        TeamId = c.Long(nullable: false),
                        AddedByID = c.Long(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AddedByID, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.TeamId, cascadeDelete: true)
                .ForeignKey("dbo.Tournaments", t => t.TournamentId, cascadeDelete: true)
                .Index(t => t.TournamentId)
                .Index(t => t.TeamId)
                .Index(t => t.AddedByID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.JoinTournamentRequests", "TournamentId", "dbo.Tournaments");
            DropForeignKey("dbo.JoinTournamentRequests", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.JoinTournamentRequests", "AddedByID", "dbo.Accounts");
            DropIndex("dbo.JoinTournamentRequests", new[] { "AddedByID" });
            DropIndex("dbo.JoinTournamentRequests", new[] { "TeamId" });
            DropIndex("dbo.JoinTournamentRequests", new[] { "TournamentId" });
            DropTable("dbo.JoinTournamentRequests");
        }
    }
}

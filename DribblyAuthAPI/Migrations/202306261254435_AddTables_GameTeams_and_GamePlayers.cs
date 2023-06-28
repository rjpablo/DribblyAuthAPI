namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTables_GameTeams_and_GamePlayers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GamePlayers",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PlayerId = c.Long(nullable: false),
                        GameId = c.Long(nullable: false),
                        Name = c.String(),
                        JerseyNo = c.Int(nullable: false),
                        Points = c.Int(nullable: false),
                        Rebounds = c.Int(nullable: false),
                        Fouls = c.Int(nullable: false),
                        Assists = c.Int(nullable: false),
                        GameTeamId = c.Long(nullable: false),
                        IsEjected = c.Boolean(nullable: false),
                        HasFouledOut = c.Boolean(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                        ProfilePhoto_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Games", t => t.GameId, cascadeDelete: true)
                .ForeignKey("dbo.GameTeams", t => t.GameTeamId, cascadeDelete: true)
                .ForeignKey("dbo.TeamMemberships", t => t.PlayerId, cascadeDelete: true)
                .ForeignKey("dbo.Photos", t => t.ProfilePhoto_Id)
                .Index(t => t.PlayerId)
                .Index(t => t.GameId)
                .Index(t => t.GameTeamId)
                .Index(t => t.ProfilePhoto_Id);
            
            CreateTable(
                "dbo.GameTeams",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        GameId = c.Long(nullable: false),
                        TeamId = c.Long(nullable: false),
                        TeamFoulCount = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Games", t => t.GameId, cascadeDelete: false)
                .ForeignKey("dbo.Teams", t => t.TeamId, cascadeDelete: false)
                .Index(t => t.GameId)
                .Index(t => t.TeamId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GamePlayers", "ProfilePhoto_Id", "dbo.Photos");
            DropForeignKey("dbo.GamePlayers", "PlayerId", "dbo.TeamMemberships");
            DropForeignKey("dbo.GamePlayers", "GameTeamId", "dbo.GameTeams");
            DropForeignKey("dbo.GameTeams", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.GameTeams", "GameId", "dbo.Games");
            DropForeignKey("dbo.GamePlayers", "GameId", "dbo.Games");
            DropIndex("dbo.GameTeams", new[] { "TeamId" });
            DropIndex("dbo.GameTeams", new[] { "GameId" });
            DropIndex("dbo.GamePlayers", new[] { "ProfilePhoto_Id" });
            DropIndex("dbo.GamePlayers", new[] { "GameTeamId" });
            DropIndex("dbo.GamePlayers", new[] { "GameId" });
            DropIndex("dbo.GamePlayers", new[] { "PlayerId" });
            DropTable("dbo.GameTeams");
            DropTable("dbo.GamePlayers");
        }
    }
}

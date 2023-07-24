namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTables_TournamentStages_StageTeams_and_StageBrackets : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StageBracketModels",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        StageId = c.Long(nullable: false),
                        Name = c.String(),
                        AddedById = c.Long(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AddedById, cascadeDelete: true)
                .ForeignKey("dbo.TournamentStages", t => t.StageId, cascadeDelete: true)
                .Index(t => t.StageId)
                .Index(t => t.AddedById);
            
            CreateTable(
                "dbo.TournamentStages",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TournamentId = c.Long(nullable: false),
                        AddedById = c.Long(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        Status = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AddedById, cascadeDelete: false)
                .ForeignKey("dbo.Tournaments", t => t.TournamentId, cascadeDelete: true)
                .Index(t => t.TournamentId)
                .Index(t => t.AddedById);
            
            CreateTable(
                "dbo.StageTeams",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        StageId = c.Long(nullable: false),
                        BracketId = c.Long(),
                        TeamId = c.Long(nullable: false),
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
                .ForeignKey("dbo.StageBracketModels", t => t.BracketId)
                .ForeignKey("dbo.TournamentStages", t => t.StageId, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.TeamId, cascadeDelete: true)
                .Index(t => t.StageId)
                .Index(t => t.BracketId)
                .Index(t => t.TeamId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StageTeams", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.StageTeams", "StageId", "dbo.TournamentStages");
            DropForeignKey("dbo.StageTeams", "BracketId", "dbo.StageBracketModels");
            DropForeignKey("dbo.StageBracketModels", "StageId", "dbo.TournamentStages");
            DropForeignKey("dbo.TournamentStages", "TournamentId", "dbo.Tournaments");
            DropForeignKey("dbo.TournamentStages", "AddedById", "dbo.Accounts");
            DropForeignKey("dbo.StageBracketModels", "AddedById", "dbo.Accounts");
            DropIndex("dbo.StageTeams", new[] { "TeamId" });
            DropIndex("dbo.StageTeams", new[] { "BracketId" });
            DropIndex("dbo.StageTeams", new[] { "StageId" });
            DropIndex("dbo.TournamentStages", new[] { "AddedById" });
            DropIndex("dbo.TournamentStages", new[] { "TournamentId" });
            DropIndex("dbo.StageBracketModels", new[] { "AddedById" });
            DropIndex("dbo.StageBracketModels", new[] { "StageId" });
            DropTable("dbo.StageTeams");
            DropTable("dbo.TournamentStages");
            DropTable("dbo.StageBracketModels");
        }
    }
}

namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTables_MemberFouls_Fouls_and_GameEvents : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Fouls",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Type = c.Int(nullable: false),
                        IsOffensive = c.Boolean(nullable: false),
                        IsDefensive = c.Boolean(nullable: false),
                        IsTechnical = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GameEvents",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        GameId = c.Long(nullable: false),
                        TeamId = c.Long(nullable: false),
                        PerformedById = c.Long(nullable: false),
                        Type = c.Int(nullable: false),
                        AdditionalData = c.String(),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Games", t => t.GameId, cascadeDelete: true)
                .ForeignKey("dbo.Accounts", t => t.PerformedById, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.TeamId, cascadeDelete: true)
                .Index(t => t.GameId)
                .Index(t => t.TeamId)
                .Index(t => t.PerformedById);
            
            CreateTable(
                "dbo.MemberFouls",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        FoulId = c.Int(nullable: false),
                        IsOffensive = c.Boolean(nullable: false),
                        IsTechnical = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GameEvents", t => t.Id)
                .ForeignKey("dbo.Fouls", t => t.FoulId, cascadeDelete: true)
                .Index(t => t.Id)
                .Index(t => t.FoulId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MemberFouls", "FoulId", "dbo.Fouls");
            DropForeignKey("dbo.MemberFouls", "Id", "dbo.GameEvents");
            DropForeignKey("dbo.GameEvents", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.GameEvents", "PerformedById", "dbo.Accounts");
            DropForeignKey("dbo.GameEvents", "GameId", "dbo.Games");
            DropIndex("dbo.MemberFouls", new[] { "FoulId" });
            DropIndex("dbo.MemberFouls", new[] { "Id" });
            DropIndex("dbo.GameEvents", new[] { "PerformedById" });
            DropIndex("dbo.GameEvents", new[] { "TeamId" });
            DropIndex("dbo.GameEvents", new[] { "GameId" });
            DropTable("dbo.MemberFouls");
            DropTable("dbo.GameEvents");
            DropTable("dbo.Fouls");
        }
    }
}

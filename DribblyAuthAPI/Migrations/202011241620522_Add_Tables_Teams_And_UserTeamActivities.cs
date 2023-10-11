namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Tables_Teams_And_UserTeamActivities : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.CourtFollowings", newName: "CourtFollowingModels");
            CreateTable(
                "dbo.Teams",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        LogoId = c.Long(),
                        AddedById = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                        HomeCourtId = c.Long(),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Courts", t => t.HomeCourtId)
                .ForeignKey("dbo.Photos", t => t.LogoId)
                .Index(t => t.LogoId)
                .Index(t => t.HomeCourtId);
            
            CreateTable(
                "dbo.UserTeamActivities",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        TeamId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserActivities", t => t.Id)
                .ForeignKey("dbo.Teams", t => t.TeamId)
                .Index(t => t.Id)
                .Index(t => t.TeamId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserTeamActivities", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.UserTeamActivities", "Id", "dbo.UserActivities");
            DropForeignKey("dbo.Teams", "LogoId", "dbo.Photos");
            DropForeignKey("dbo.Teams", "HomeCourtId", "dbo.Courts");
            DropIndex("dbo.UserTeamActivities", new[] { "TeamId" });
            DropIndex("dbo.UserTeamActivities", new[] { "Id" });
            DropIndex("dbo.Teams", new[] { "HomeCourtId" });
            DropIndex("dbo.Teams", new[] { "LogoId" });
            DropTable("dbo.UserTeamActivities");
            DropTable("dbo.Teams");
            RenameTable(name: "dbo.CourtFollowingModels", newName: "CourtFollowings");
        }
    }
}

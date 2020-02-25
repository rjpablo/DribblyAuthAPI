namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_GameModelAndEventModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                        Title = c.String(),
                        AddedBy = c.String(),
                        CourtId = c.Long(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Courts", t => t.CourtId, cascadeDelete: true)
                .Index(t => t.CourtId);
            
            CreateTable(
                "dbo.Games",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        Team1Id = c.Long(nullable: false),
                        Team2Id = c.Long(nullable: false),
                        WinningTeamId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Events", t => t.Id)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Games", "Id", "dbo.Events");
            DropForeignKey("dbo.Events", "CourtId", "dbo.Courts");
            DropIndex("dbo.Games", new[] { "Id" });
            DropIndex("dbo.Events", new[] { "CourtId" });
            DropTable("dbo.Games");
            DropTable("dbo.Events");
        }
    }
}

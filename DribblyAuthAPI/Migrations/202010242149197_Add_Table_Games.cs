namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Table_Games : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Games",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Start = c.DateTime(nullable: true),
                        End = c.DateTime(nullable: true),
                        Title = c.String(),
                        AddedById = c.String(nullable: false),
                        CourtId = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                        Team1Id = c.Long(nullable: true),
                        Team2Id = c.Long(nullable: true),
                        WinningTeamId = c.Long(nullable: true),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Courts", t => t.CourtId, cascadeDelete: true)
                .Index(t => t.CourtId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Games", "CourtId", "dbo.Courts");
            DropIndex("dbo.Games", new[] { "CourtId" });
            DropTable("dbo.Games");
        }
    }
}

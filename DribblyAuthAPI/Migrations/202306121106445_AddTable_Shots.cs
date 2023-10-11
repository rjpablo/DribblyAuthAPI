namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTable_Shots : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Shots",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Points = c.Int(nullable: false),
                        IsMiss = c.Boolean(nullable: false),
                        TakenById = c.Long(),
                        TeamId = c.Long(nullable: false),
                        GameId = c.Long(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Games", t => t.GameId, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.TeamId, cascadeDelete: true)
                .Index(t => t.TeamId)
                .Index(t => t.GameId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Shots", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.Shots", "GameId", "dbo.Games");
            DropIndex("dbo.Shots", new[] { "GameId" });
            DropIndex("dbo.Shots", new[] { "TeamId" });
            DropTable("dbo.Shots");
        }
    }
}

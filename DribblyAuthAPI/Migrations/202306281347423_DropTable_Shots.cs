namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropTable_Shots : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Shots", "GameId", "dbo.Games");
            DropForeignKey("dbo.Shots", "TeamId", "dbo.Teams");
            DropIndex("dbo.Shots", new[] { "TeamId" });
            DropIndex("dbo.Shots", new[] { "GameId" });
            DropTable("dbo.Shots");
        }
        
        public override void Down()
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
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.Shots", "GameId");
            CreateIndex("dbo.Shots", "TeamId");
            AddForeignKey("dbo.Shots", "TeamId", "dbo.Teams", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Shots", "GameId", "dbo.Games", "Id", cascadeDelete: true);
        }
    }
}

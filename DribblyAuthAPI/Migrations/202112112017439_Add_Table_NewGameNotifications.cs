namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Table_NewGameNotifications : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NewGameNotifications",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        GameId = c.Long(nullable: false),
                        BookedById = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Notifications", t => t.Id)
                .ForeignKey("dbo.Games", t => t.GameId, cascadeDelete: true)
                .ForeignKey("dbo.Accounts", t => t.BookedById, cascadeDelete: true)
                .Index(t => t.Id)
                .Index(t => t.GameId)
                .Index(t => t.BookedById);
            
            CreateTable(
                "dbo.UpdateGameNotifications",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        GameId = c.Long(nullable: false),
                        UpdatedById = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Notifications", t => t.Id)
                .ForeignKey("dbo.Games", t => t.GameId, cascadeDelete: true)
                .ForeignKey("dbo.Accounts", t => t.UpdatedById, cascadeDelete: true)
                .Index(t => t.Id)
                .Index(t => t.GameId)
                .Index(t => t.UpdatedById);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UpdateGameNotifications", "UpdatedById", "dbo.Accounts");
            DropForeignKey("dbo.UpdateGameNotifications", "GameId", "dbo.Games");
            DropForeignKey("dbo.UpdateGameNotifications", "Id", "dbo.Notifications");
            DropForeignKey("dbo.NewGameNotifications", "BookedById", "dbo.Accounts");
            DropForeignKey("dbo.NewGameNotifications", "GameId", "dbo.Games");
            DropForeignKey("dbo.NewGameNotifications", "Id", "dbo.Notifications");
            DropIndex("dbo.UpdateGameNotifications", new[] { "UpdatedById" });
            DropIndex("dbo.UpdateGameNotifications", new[] { "GameId" });
            DropIndex("dbo.UpdateGameNotifications", new[] { "Id" });
            DropIndex("dbo.NewGameNotifications", new[] { "BookedById" });
            DropIndex("dbo.NewGameNotifications", new[] { "GameId" });
            DropIndex("dbo.NewGameNotifications", new[] { "Id" });
            DropTable("dbo.UpdateGameNotifications");
            DropTable("dbo.NewGameNotifications");
        }
    }
}

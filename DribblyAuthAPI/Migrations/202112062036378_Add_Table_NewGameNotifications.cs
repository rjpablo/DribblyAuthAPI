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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NewGameNotifications", "BookedById", "dbo.Accounts");
            DropForeignKey("dbo.NewGameNotifications", "GameId", "dbo.Games");
            DropForeignKey("dbo.NewGameNotifications", "Id", "dbo.Notifications");
            DropIndex("dbo.NewGameNotifications", new[] { "BookedById" });
            DropIndex("dbo.NewGameNotifications", new[] { "GameId" });
            DropIndex("dbo.NewGameNotifications", new[] { "Id" });
            DropTable("dbo.NewGameNotifications");
        }
    }
}

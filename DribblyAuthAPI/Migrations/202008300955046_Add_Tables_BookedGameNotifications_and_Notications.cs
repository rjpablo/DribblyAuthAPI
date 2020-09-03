namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Tables_BookedGameNotifications_and_Notications : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ForUserId = c.String(),
                        Type = c.Int(nullable: false),
                        IsViewed = c.Boolean(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GameBookedNotifications",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        GameId = c.Long(nullable: false),
                        BookedById = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Notifications", t => t.Id)
                .ForeignKey("dbo.Games", t => t.GameId)
                .Index(t => t.Id)
                .Index(t => t.GameId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GameBookedNotifications", "GameId", "dbo.Games");
            DropForeignKey("dbo.GameBookedNotifications", "Id", "dbo.Notifications");
            DropIndex("dbo.GameBookedNotifications", new[] { "GameId" });
            DropIndex("dbo.GameBookedNotifications", new[] { "Id" });
            DropTable("dbo.GameBookedNotifications");
            DropTable("dbo.Notifications");
        }
    }
}

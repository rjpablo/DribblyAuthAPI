namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Tables_UserAccountActivities_And_AccountPhotoActivities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserAccountActivities",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        AccountId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserActivities", t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AccountId, cascadeDelete: true)
                .Index(t => t.Id)
                .Index(t => t.AccountId);
            
            CreateTable(
                "dbo.AccountPhotoActivities",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        PhotoId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserAccountActivities", t => t.Id)
                .ForeignKey("dbo.Photos", t => t.PhotoId, cascadeDelete: true)
                .Index(t => t.Id)
                .Index(t => t.PhotoId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccountPhotoActivities", "PhotoId", "dbo.Photos");
            DropForeignKey("dbo.AccountPhotoActivities", "Id", "dbo.UserAccountActivities");
            DropForeignKey("dbo.UserAccountActivities", "AccountId", "dbo.Accounts");
            DropForeignKey("dbo.UserAccountActivities", "Id", "dbo.UserActivities");
            DropIndex("dbo.AccountPhotoActivities", new[] { "PhotoId" });
            DropIndex("dbo.AccountPhotoActivities", new[] { "Id" });
            DropIndex("dbo.UserAccountActivities", new[] { "AccountId" });
            DropIndex("dbo.UserAccountActivities", new[] { "Id" });
            DropTable("dbo.AccountPhotoActivities");
            DropTable("dbo.UserAccountActivities");
        }
    }
}

namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAccountPhotos : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountPhotos",
                c => new
                    {
                        PhotoId = c.Long(nullable: false),
                        AccountId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.PhotoId, t.AccountId })
                .ForeignKey("dbo.Accounts", t => t.AccountId, cascadeDelete: true)
                .ForeignKey("dbo.Photos", t => t.PhotoId, cascadeDelete: true)
                .Index(t => t.PhotoId)
                .Index(t => t.AccountId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccountPhotos", "PhotoId", "dbo.Photos");
            DropForeignKey("dbo.AccountPhotos", "AccountId", "dbo.Accounts");
            DropIndex("dbo.AccountPhotos", new[] { "AccountId" });
            DropIndex("dbo.AccountPhotos", new[] { "PhotoId" });
            DropTable("dbo.AccountPhotos");
        }
    }
}

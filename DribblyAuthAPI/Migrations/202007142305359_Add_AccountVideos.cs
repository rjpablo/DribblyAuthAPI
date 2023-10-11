namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_AccountVideos : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountVideos",
                c => new
                    {
                        VideoId = c.Long(nullable: false),
                        AccountId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.VideoId, t.AccountId })
                .ForeignKey("dbo.Accounts", t => t.AccountId, cascadeDelete: true)
                .ForeignKey("dbo.Videos", t => t.VideoId, cascadeDelete: true)
                .Index(t => t.VideoId)
                .Index(t => t.AccountId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccountVideos", "VideoId", "dbo.Videos");
            DropForeignKey("dbo.AccountVideos", "AccountId", "dbo.Accounts");
            DropIndex("dbo.AccountVideos", new[] { "AccountId" });
            DropIndex("dbo.AccountVideos", new[] { "VideoId" });
            DropTable("dbo.AccountVideos");
        }
    }
}

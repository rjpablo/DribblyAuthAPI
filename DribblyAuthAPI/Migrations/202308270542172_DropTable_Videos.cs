namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropTable_Videos : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CourtVideos", "VideoId", "dbo.Videos");
            DropForeignKey("dbo.AccountVideos", "VideoId", "dbo.Videos");
            DropForeignKey("dbo.AccountVideoActivities", "VideoId", "dbo.Videos");
            DropForeignKey("dbo.CourtVideoActivities", "VideoId", "dbo.Videos");
            DropForeignKey("dbo.Videos", "Id", "dbo.Multimedia");
            DropIndex("dbo.CourtVideos", new[] { "VideoId" });
            DropIndex("dbo.AccountVideos", new[] { "VideoId" });
            DropIndex("dbo.AccountVideoActivities", new[] { "VideoId" });
            DropIndex("dbo.CourtVideoActivities", new[] { "VideoId" });
            DropIndex("dbo.Videos", new[] { "Id" });
            DropTable("dbo.Videos");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Videos",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        Description = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.Videos", "Id");
            CreateIndex("dbo.CourtVideoActivities", "VideoId");
            CreateIndex("dbo.AccountVideoActivities", "VideoId");
            CreateIndex("dbo.AccountVideos", "VideoId");
            CreateIndex("dbo.CourtVideos", "VideoId");
            AddForeignKey("dbo.Videos", "Id", "dbo.Multimedia", "Id");
            AddForeignKey("dbo.CourtVideoActivities", "VideoId", "dbo.Videos", "Id");
            AddForeignKey("dbo.AccountVideoActivities", "VideoId", "dbo.Videos", "Id");
            AddForeignKey("dbo.AccountVideos", "VideoId", "dbo.Videos", "Id");
            AddForeignKey("dbo.CourtVideos", "VideoId", "dbo.Videos", "Id");
        }
    }
}

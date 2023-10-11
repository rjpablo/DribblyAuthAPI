namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTable_Videos : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Videos",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        Description = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Multimedia", t => t.Id)
                .Index(t => t.Id);
            
            CreateIndex("dbo.CourtVideos", "VideoId");
            CreateIndex("dbo.AccountVideos", "VideoId");
            CreateIndex("dbo.AccountVideoActivities", "VideoId");
            CreateIndex("dbo.CourtVideoActivities", "VideoId");
            AddForeignKey("dbo.CourtVideos", "VideoId", "dbo.Videos", "Id");
            AddForeignKey("dbo.AccountVideos", "VideoId", "dbo.Videos", "Id");
            AddForeignKey("dbo.AccountVideoActivities", "VideoId", "dbo.Videos", "Id");
            AddForeignKey("dbo.CourtVideoActivities", "VideoId", "dbo.Videos", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Videos", "Id", "dbo.Multimedia");
            DropForeignKey("dbo.CourtVideoActivities", "VideoId", "dbo.Videos");
            DropForeignKey("dbo.AccountVideoActivities", "VideoId", "dbo.Videos");
            DropForeignKey("dbo.AccountVideos", "VideoId", "dbo.Videos");
            DropForeignKey("dbo.CourtVideos", "VideoId", "dbo.Videos");
            DropIndex("dbo.Videos", new[] { "Id" });
            DropIndex("dbo.CourtVideoActivities", new[] { "VideoId" });
            DropIndex("dbo.AccountVideoActivities", new[] { "VideoId" });
            DropIndex("dbo.AccountVideos", new[] { "VideoId" });
            DropIndex("dbo.CourtVideos", new[] { "VideoId" });
            DropTable("dbo.Videos");
        }
    }
}

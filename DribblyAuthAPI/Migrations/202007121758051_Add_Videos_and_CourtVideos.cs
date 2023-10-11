namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Videos_and_CourtVideos : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CourtVideos",
                c => new
                    {
                        VideoId = c.Long(nullable: false),
                        CourtId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.VideoId, t.CourtId })
                .ForeignKey("dbo.Courts", t => t.CourtId, cascadeDelete: true)
                .ForeignKey("dbo.Videos", t => t.VideoId, cascadeDelete: true)
                .Index(t => t.VideoId)
                .Index(t => t.CourtId);
            
            CreateTable(
                "dbo.Videos",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Src = c.String(),
                        Title = c.String(maxLength: 100),
                        Description = c.String(maxLength: 2000),
                        AddedBy = c.String(),
                        Size = c.Long(nullable: false),
                        Type = c.String(),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CourtVideos", "VideoId", "dbo.Videos");
            DropForeignKey("dbo.CourtVideos", "CourtId", "dbo.Courts");
            DropIndex("dbo.CourtVideos", new[] { "CourtId" });
            DropIndex("dbo.CourtVideos", new[] { "VideoId" });
            DropTable("dbo.Videos");
            DropTable("dbo.CourtVideos");
        }
    }
}

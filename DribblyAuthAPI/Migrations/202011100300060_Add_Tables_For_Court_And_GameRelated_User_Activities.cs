namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Tables_For_Court_And_GameRelated_User_Activities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CourtPhotoActivities",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        PhotoId = c.Long(nullable: false),
                        CourtId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserActivities", t => t.Id)
                .ForeignKey("dbo.Photos", t => t.PhotoId, cascadeDelete: true)
                .ForeignKey("dbo.Courts", t => t.CourtId, cascadeDelete: true)
                .Index(t => t.Id)
                .Index(t => t.PhotoId)
                .Index(t => t.CourtId);
            
            CreateTable(
                "dbo.CourtVideoActivities",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        VideoId = c.Long(nullable: false),
                        CourtId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserActivities", t => t.Id)
                .ForeignKey("dbo.Videos", t => t.VideoId, cascadeDelete: true)
                .ForeignKey("dbo.Courts", t => t.CourtId, cascadeDelete: true)
                .Index(t => t.Id)
                .Index(t => t.VideoId)
                .Index(t => t.CourtId);
            
            CreateTable(
                "dbo.UserCourtActivities",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        CourtId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserActivities", t => t.Id)
                .ForeignKey("dbo.Courts", t => t.CourtId, cascadeDelete: true)
                .Index(t => t.Id)
                .Index(t => t.CourtId);
            
            CreateTable(
                "dbo.UserGameActivities",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        GameId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserActivities", t => t.Id)
                .ForeignKey("dbo.Games", t => t.GameId)
                .Index(t => t.Id)
                .Index(t => t.GameId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserGameActivities", "GameId", "dbo.Games");
            DropForeignKey("dbo.UserGameActivities", "Id", "dbo.UserActivities");
            DropForeignKey("dbo.UserCourtActivities", "CourtId", "dbo.Courts");
            DropForeignKey("dbo.UserCourtActivities", "Id", "dbo.UserActivities");
            DropForeignKey("dbo.CourtVideoActivities", "CourtId", "dbo.Courts");
            DropForeignKey("dbo.CourtVideoActivities", "VideoId", "dbo.Videos");
            DropForeignKey("dbo.CourtVideoActivities", "Id", "dbo.UserActivities");
            DropForeignKey("dbo.CourtPhotoActivities", "CourtId", "dbo.Courts");
            DropForeignKey("dbo.CourtPhotoActivities", "PhotoId", "dbo.Photos");
            DropForeignKey("dbo.CourtPhotoActivities", "Id", "dbo.UserActivities");
            DropIndex("dbo.UserGameActivities", new[] { "GameId" });
            DropIndex("dbo.UserGameActivities", new[] { "Id" });
            DropIndex("dbo.UserCourtActivities", new[] { "CourtId" });
            DropIndex("dbo.UserCourtActivities", new[] { "Id" });
            DropIndex("dbo.CourtVideoActivities", new[] { "CourtId" });
            DropIndex("dbo.CourtVideoActivities", new[] { "VideoId" });
            DropIndex("dbo.CourtVideoActivities", new[] { "Id" });
            DropIndex("dbo.CourtPhotoActivities", new[] { "CourtId" });
            DropIndex("dbo.CourtPhotoActivities", new[] { "PhotoId" });
            DropIndex("dbo.CourtPhotoActivities", new[] { "Id" });
            DropTable("dbo.UserGameActivities");
            DropTable("dbo.UserCourtActivities");
            DropTable("dbo.CourtVideoActivities");
            DropTable("dbo.CourtPhotoActivities");
        }
    }
}

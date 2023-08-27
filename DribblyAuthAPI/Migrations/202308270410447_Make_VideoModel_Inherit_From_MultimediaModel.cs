namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Make_VideoModel_Inherit_From_MultimediaModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CourtVideos", "VideoId", "dbo.Videos");
            DropForeignKey("dbo.AccountVideos", "VideoId", "dbo.Videos");
            DropForeignKey("dbo.AccountVideoActivities", "VideoId", "dbo.Videos");
            DropForeignKey("dbo.CourtVideoActivities", "VideoId", "dbo.Videos");
            DropPrimaryKey("dbo.Videos");
            AddColumn("dbo.Multimedia", "Title", c => c.String(maxLength: 100));
            AddColumn("dbo.Multimedia", "Size", c => c.Long(nullable: false));
            AlterColumn("dbo.Videos", "Id", c => c.Long(nullable: false));
            AddPrimaryKey("dbo.Videos", "Id");
            CreateIndex("dbo.Videos", "Id");
            AddForeignKey("dbo.Videos", "Id", "dbo.Multimedia", "Id");
            AddForeignKey("dbo.CourtVideos", "VideoId", "dbo.Videos", "Id");
            AddForeignKey("dbo.AccountVideos", "VideoId", "dbo.Videos", "Id");
            AddForeignKey("dbo.AccountVideoActivities", "VideoId", "dbo.Videos", "Id");
            AddForeignKey("dbo.CourtVideoActivities", "VideoId", "dbo.Videos", "Id");
            DropColumn("dbo.Videos", "Src");
            DropColumn("dbo.Videos", "Title");
            DropColumn("dbo.Videos", "AddedBy");
            DropColumn("dbo.Videos", "Size");
            DropColumn("dbo.Videos", "Type");
            DropColumn("dbo.Videos", "DateDeleted");
            DropColumn("dbo.Videos", "DateAdded");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Videos", "DateAdded", c => c.DateTime(nullable: false));
            AddColumn("dbo.Videos", "DateDeleted", c => c.DateTime());
            AddColumn("dbo.Videos", "Type", c => c.String());
            AddColumn("dbo.Videos", "Size", c => c.Long(nullable: false));
            AddColumn("dbo.Videos", "AddedBy", c => c.Long(nullable: false));
            AddColumn("dbo.Videos", "Title", c => c.String(maxLength: 100));
            AddColumn("dbo.Videos", "Src", c => c.String());
            DropForeignKey("dbo.CourtVideoActivities", "VideoId", "dbo.Videos");
            DropForeignKey("dbo.AccountVideoActivities", "VideoId", "dbo.Videos");
            DropForeignKey("dbo.AccountVideos", "VideoId", "dbo.Videos");
            DropForeignKey("dbo.CourtVideos", "VideoId", "dbo.Videos");
            DropForeignKey("dbo.Videos", "Id", "dbo.Multimedia");
            DropIndex("dbo.Videos", new[] { "Id" });
            DropPrimaryKey("dbo.Videos");
            AlterColumn("dbo.Videos", "Id", c => c.Long(nullable: false, identity: true));
            DropColumn("dbo.Multimedia", "Size");
            DropColumn("dbo.Multimedia", "Title");
            AddPrimaryKey("dbo.Videos", "Id");
            AddForeignKey("dbo.CourtVideoActivities", "VideoId", "dbo.Videos", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AccountVideoActivities", "VideoId", "dbo.Videos", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AccountVideos", "VideoId", "dbo.Videos", "Id", cascadeDelete: true);
            AddForeignKey("dbo.CourtVideos", "VideoId", "dbo.Videos", "Id", cascadeDelete: true);
        }
    }
}

namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Table_TeamPhotoActivities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TeamPhotoActivities",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        PhotoId = c.Long(nullable: false),
                        TeamId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserActivities", t => t.Id)
                .ForeignKey("dbo.Photos", t => t.PhotoId, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.TeamId, cascadeDelete: true)
                .Index(t => t.Id)
                .Index(t => t.PhotoId)
                .Index(t => t.TeamId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TeamPhotoActivities", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.TeamPhotoActivities", "PhotoId", "dbo.Photos");
            DropForeignKey("dbo.TeamPhotoActivities", "Id", "dbo.UserActivities");
            DropIndex("dbo.TeamPhotoActivities", new[] { "TeamId" });
            DropIndex("dbo.TeamPhotoActivities", new[] { "PhotoId" });
            DropIndex("dbo.TeamPhotoActivities", new[] { "Id" });
            DropTable("dbo.TeamPhotoActivities");
        }
    }
}

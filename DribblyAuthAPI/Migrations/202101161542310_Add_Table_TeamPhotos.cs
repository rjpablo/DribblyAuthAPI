namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Table_TeamPhotos : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TeamPhotos",
                c => new
                    {
                        PhotoId = c.Long(nullable: false),
                        TeamId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.PhotoId, t.TeamId })
                .ForeignKey("dbo.Photos", t => t.PhotoId, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.TeamId, cascadeDelete: true)
                .Index(t => t.PhotoId)
                .Index(t => t.TeamId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TeamPhotos", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.TeamPhotos", "PhotoId", "dbo.Photos");
            DropIndex("dbo.TeamPhotos", new[] { "TeamId" });
            DropIndex("dbo.TeamPhotos", new[] { "PhotoId" });
            DropTable("dbo.TeamPhotos");
        }
    }
}

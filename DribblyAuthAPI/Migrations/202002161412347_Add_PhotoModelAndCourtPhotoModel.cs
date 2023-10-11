namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_PhotoModelAndCourtPhotoModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CourtPhotos",
                c => new
                    {
                        PhotoId = c.Long(nullable: false),
                        CourtId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.PhotoId, t.CourtId })
                .ForeignKey("dbo.Courts", t => t.CourtId, cascadeDelete: true)
                .ForeignKey("dbo.Photos", t => t.PhotoId, cascadeDelete: true)
                .Index(t => t.PhotoId)
                .Index(t => t.CourtId);
            
            CreateTable(
                "dbo.Photos",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Url = c.String(),
                        UploadedById = c.String(),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CourtPhotos", "PhotoId", "dbo.Photos");
            DropForeignKey("dbo.CourtPhotos", "CourtId", "dbo.Courts");
            DropIndex("dbo.CourtPhotos", new[] { "CourtId" });
            DropIndex("dbo.CourtPhotos", new[] { "PhotoId" });
            DropTable("dbo.Photos");
            DropTable("dbo.CourtPhotos");
        }
    }
}

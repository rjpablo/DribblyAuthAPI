namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modify_CourtModel_RemovePrimaryPhotoUrlAndAddPrimaryPhotoId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courts", "PrimaryPhotoId", c => c.Long());
            CreateIndex("dbo.Courts", "PrimaryPhotoId");
            AddForeignKey("dbo.Courts", "PrimaryPhotoId", "dbo.Photos", "Id");
            DropColumn("dbo.Courts", "PrimaryPhotoUrl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Courts", "PrimaryPhotoUrl", c => c.String());
            DropForeignKey("dbo.Courts", "PrimaryPhotoId", "dbo.Photos");
            DropIndex("dbo.Courts", new[] { "PrimaryPhotoId" });
            DropColumn("dbo.Courts", "PrimaryPhotoId");
        }
    }
}

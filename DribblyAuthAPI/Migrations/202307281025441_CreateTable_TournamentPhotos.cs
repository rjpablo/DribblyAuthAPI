namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTable_TournamentPhotos : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TournamentPhotos",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TournamentId = c.Long(nullable: false),
                        PhotoId = c.Long(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Photos", t => t.PhotoId, cascadeDelete: true)
                .ForeignKey("dbo.Tournaments", t => t.TournamentId, cascadeDelete: true)
                .Index(t => t.TournamentId)
                .Index(t => t.PhotoId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TournamentPhotos", "TournamentId", "dbo.Tournaments");
            DropForeignKey("dbo.TournamentPhotos", "PhotoId", "dbo.Photos");
            DropIndex("dbo.TournamentPhotos", new[] { "PhotoId" });
            DropIndex("dbo.TournamentPhotos", new[] { "TournamentId" });
            DropTable("dbo.TournamentPhotos");
        }
    }
}

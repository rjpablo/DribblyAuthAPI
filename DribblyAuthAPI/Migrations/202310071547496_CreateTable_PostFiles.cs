namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTable_PostFiles : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PostFiles",
                c => new
                    {
                        PostId = c.Long(nullable: false),
                        FileId = c.Long(nullable: false),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PostId, t.FileId })
                .ForeignKey("dbo.Multimedia", t => t.FileId, cascadeDelete: true)
                .ForeignKey("dbo.Posts", t => t.PostId, cascadeDelete: true)
                .Index(t => t.PostId)
                .Index(t => t.FileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PostFiles", "PostId", "dbo.Posts");
            DropForeignKey("dbo.PostFiles", "FileId", "dbo.Multimedia");
            DropIndex("dbo.PostFiles", new[] { "FileId" });
            DropIndex("dbo.PostFiles", new[] { "PostId" });
            DropTable("dbo.PostFiles");
        }
    }
}

namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTable_AccountHighlights : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountHighlights",
                c => new
                    {
                        AccountId = c.Long(nullable: false),
                        FileId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.AccountId, t.FileId })
                .ForeignKey("dbo.Players", t => t.AccountId)
                .ForeignKey("dbo.Multimedia", t => t.FileId, cascadeDelete: true)
                .Index(t => t.AccountId)
                .Index(t => t.FileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccountHighlights", "FileId", "dbo.Multimedia");
            DropForeignKey("dbo.AccountHighlights", "AccountId", "dbo.Players");
            DropIndex("dbo.AccountHighlights", new[] { "FileId" });
            DropIndex("dbo.AccountHighlights", new[] { "AccountId" });
            DropTable("dbo.AccountHighlights");
        }
    }
}

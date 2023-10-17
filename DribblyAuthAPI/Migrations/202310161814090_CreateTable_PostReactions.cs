namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTable_PostReactions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PostReactions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PostId = c.Long(nullable: false),
                        Type = c.Int(nullable: false),
                        ReactorId = c.Long(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Posts", t => t.PostId, cascadeDelete: true)
                .ForeignKey("dbo.Accounts", t => t.ReactorId, cascadeDelete: true)
                .Index(t => t.PostId)
                .Index(t => t.ReactorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PostReactions", "ReactorId", "dbo.Accounts");
            DropForeignKey("dbo.PostReactions", "PostId", "dbo.Posts");
            DropIndex("dbo.PostReactions", new[] { "ReactorId" });
            DropIndex("dbo.PostReactions", new[] { "PostId" });
            DropTable("dbo.PostReactions");
        }
    }
}

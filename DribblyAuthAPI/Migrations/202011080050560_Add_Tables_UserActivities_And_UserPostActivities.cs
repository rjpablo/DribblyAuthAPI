namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Tables_UserActivities_And_UserPostActivities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserActivities",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        UserId = c.String(),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserPostActivities",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        PostId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserActivities", t => t.Id)
                .ForeignKey("dbo.Posts", t => t.PostId, cascadeDelete: true)
                .Index(t => t.Id)
                .Index(t => t.PostId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserPostActivities", "PostId", "dbo.Posts");
            DropForeignKey("dbo.UserPostActivities", "Id", "dbo.UserActivities");
            DropIndex("dbo.UserPostActivities", new[] { "PostId" });
            DropIndex("dbo.UserPostActivities", new[] { "Id" });
            DropTable("dbo.UserPostActivities");
            DropTable("dbo.UserActivities");
        }
    }
}

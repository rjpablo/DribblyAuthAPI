namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Table_AccountVideoActivities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountVideoActivities",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        VideoId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserAccountActivities", t => t.Id)
                .ForeignKey("dbo.Videos", t => t.VideoId, cascadeDelete: true)
                .Index(t => t.Id)
                .Index(t => t.VideoId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccountVideoActivities", "VideoId", "dbo.Videos");
            DropForeignKey("dbo.AccountVideoActivities", "Id", "dbo.UserAccountActivities");
            DropIndex("dbo.AccountVideoActivities", new[] { "VideoId" });
            DropIndex("dbo.AccountVideoActivities", new[] { "Id" });
            DropTable("dbo.AccountVideoActivities");
        }
    }
}

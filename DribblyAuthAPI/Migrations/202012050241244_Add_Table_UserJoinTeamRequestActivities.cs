namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Table_UserJoinTeamRequestActivities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserJoinTeamRequestActivities",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        RequestId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserActivities", t => t.Id)
                .ForeignKey("dbo.JoinTeamRequests", t => t.RequestId, cascadeDelete: true)
                .Index(t => t.Id)
                .Index(t => t.RequestId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserJoinTeamRequestActivities", "RequestId", "dbo.JoinTeamRequests");
            DropForeignKey("dbo.UserJoinTeamRequestActivities", "Id", "dbo.UserActivities");
            DropIndex("dbo.UserJoinTeamRequestActivities", new[] { "RequestId" });
            DropIndex("dbo.UserJoinTeamRequestActivities", new[] { "Id" });
            DropTable("dbo.UserJoinTeamRequestActivities");
        }
    }
}

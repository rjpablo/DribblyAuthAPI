namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Table_JoinTeamRequestNotifications_And_Modify_Table_Team_Add_Column_ManagedById : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.JoinTeamRequestNotifications",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        RequestId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Notifications", t => t.Id)
                .ForeignKey("dbo.JoinTeamRequests", t => t.RequestId, cascadeDelete: true)
                .Index(t => t.Id)
                .Index(t => t.RequestId);
            
            AddColumn("dbo.Teams", "ManagedById", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.JoinTeamRequestNotifications", "RequestId", "dbo.JoinTeamRequests");
            DropForeignKey("dbo.JoinTeamRequestNotifications", "Id", "dbo.Notifications");
            DropIndex("dbo.JoinTeamRequestNotifications", new[] { "RequestId" });
            DropIndex("dbo.JoinTeamRequestNotifications", new[] { "Id" });
            DropColumn("dbo.Teams", "ManagedById");
            DropTable("dbo.JoinTeamRequestNotifications");
        }
    }
}

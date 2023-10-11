namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Tables_TeamMembers_And_JoinTeamRequests : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.JoinTeamRequests",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TeamId = c.Long(nullable: false),
                        MemberAccountId = c.Long(nullable: false),
                        Position = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.MemberAccountId, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.TeamId, cascadeDelete: true)
                .Index(t => t.TeamId)
                .Index(t => t.MemberAccountId);
            
            CreateTable(
                "dbo.TeamMemberships",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TeamId = c.Long(nullable: false),
                        MemberAccountId = c.Long(nullable: false),
                        DateLeft = c.DateTime(),
                        Position = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.MemberAccountId, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.TeamId, cascadeDelete: true)
                .Index(t => t.TeamId)
                .Index(t => t.MemberAccountId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TeamMemberships", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.TeamMemberships", "MemberAccountId", "dbo.Accounts");
            DropForeignKey("dbo.JoinTeamRequests", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.JoinTeamRequests", "MemberAccountId", "dbo.Accounts");
            DropIndex("dbo.TeamMemberships", new[] { "MemberAccountId" });
            DropIndex("dbo.TeamMemberships", new[] { "TeamId" });
            DropIndex("dbo.JoinTeamRequests", new[] { "MemberAccountId" });
            DropIndex("dbo.JoinTeamRequests", new[] { "TeamId" });
            DropTable("dbo.TeamMemberships");
            DropTable("dbo.JoinTeamRequests");
        }
    }
}

namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTable_JoinGroupRequests : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.JoinGroupRequests",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RequestorId = c.Long(nullable: false),
                        GroupId = c.Long(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("dbo.Accounts", t => t.RequestorId, cascadeDelete: false)
                .Index(t => t.RequestorId)
                .Index(t => t.GroupId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.JoinGroupRequests", "RequestorId", "dbo.Accounts");
            DropForeignKey("dbo.JoinGroupRequests", "GroupId", "dbo.Groups");
            DropIndex("dbo.JoinGroupRequests", new[] { "GroupId" });
            DropIndex("dbo.JoinGroupRequests", new[] { "RequestorId" });
            DropTable("dbo.JoinGroupRequests");
        }
    }
}

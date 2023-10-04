namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTables_Groups_and_GroupMembers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GroupMembers",
                c => new
                    {
                        AccountId = c.Long(nullable: false),
                        GroupId = c.Long(nullable: false),
                        DateJoined = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.AccountId, t.GroupId })
                .ForeignKey("dbo.Accounts", t => t.AccountId, cascadeDelete: false)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .Index(t => t.AccountId)
                .Index(t => t.GroupId);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        LogoId = c.Long(),
                        AddedById = c.Long(nullable: false),
                        EntityStatus = c.Int(nullable: false),
                        Description = c.String(),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AddedById, cascadeDelete: true)
                .ForeignKey("dbo.Multimedia", t => t.LogoId)
                .Index(t => t.LogoId)
                .Index(t => t.AddedById);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GroupMembers", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.Groups", "LogoId", "dbo.Multimedia");
            DropForeignKey("dbo.Groups", "AddedById", "dbo.Accounts");
            DropForeignKey("dbo.GroupMembers", "AccountId", "dbo.Accounts");
            DropIndex("dbo.Groups", new[] { "AddedById" });
            DropIndex("dbo.Groups", new[] { "LogoId" });
            DropIndex("dbo.GroupMembers", new[] { "GroupId" });
            DropIndex("dbo.GroupMembers", new[] { "AccountId" });
            DropTable("dbo.Groups");
            DropTable("dbo.GroupMembers");
        }
    }
}

namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTables_Events_and_EventAttendees : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EventAttendees",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AccountId = c.Long(nullable: false),
                        EventId = c.Long(nullable: false),
                        DateJoined = c.DateTime(nullable: false),
                        IsApproved = c.Boolean(nullable: false),
                        IsAdmin = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AccountId, cascadeDelete: true)
                .ForeignKey("dbo.Events", t => t.EventId, cascadeDelete: true)
                .Index(t => t.AccountId)
                .Index(t => t.EventId);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Title = c.String(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        LogoId = c.Long(),
                        EntityStatus = c.Int(nullable: false),
                        Description = c.String(),
                        AddedById = c.Long(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AddedById, cascadeDelete: false)
                .ForeignKey("dbo.Multimedia", t => t.LogoId)
                .Index(t => t.LogoId)
                .Index(t => t.AddedById);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EventAttendees", "EventId", "dbo.Events");
            DropForeignKey("dbo.Events", "LogoId", "dbo.Multimedia");
            DropForeignKey("dbo.Events", "AddedById", "dbo.Accounts");
            DropForeignKey("dbo.EventAttendees", "AccountId", "dbo.Accounts");
            DropIndex("dbo.Events", new[] { "AddedById" });
            DropIndex("dbo.Events", new[] { "LogoId" });
            DropIndex("dbo.EventAttendees", new[] { "EventId" });
            DropIndex("dbo.EventAttendees", new[] { "AccountId" });
            DropTable("dbo.Events");
            DropTable("dbo.EventAttendees");
        }
    }
}

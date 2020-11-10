namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Table_UserContactActivities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserContactActivities",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        ContactId = c.Long(),
                        ContactNo = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserActivities", t => t.Id)
                .ForeignKey("dbo.Contacts", t => t.ContactId)
                .Index(t => t.Id)
                .Index(t => t.ContactId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserContactActivities", "ContactId", "dbo.Contacts");
            DropForeignKey("dbo.UserContactActivities", "Id", "dbo.UserActivities");
            DropIndex("dbo.UserContactActivities", new[] { "ContactId" });
            DropIndex("dbo.UserContactActivities", new[] { "Id" });
            DropTable("dbo.UserContactActivities");
        }
    }
}

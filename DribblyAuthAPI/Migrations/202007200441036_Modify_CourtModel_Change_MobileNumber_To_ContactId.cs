namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modify_CourtModel_Change_MobileNumber_To_ContactId : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Number = c.String(),
                        AddedBy = c.String(),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Courts", "ContactId", c => c.Long());
            CreateIndex("dbo.Courts", "ContactId");
            AddForeignKey("dbo.Courts", "ContactId", "dbo.Contacts", "Id");
            DropColumn("dbo.Courts", "MobileNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Courts", "MobileNumber", c => c.String());
            DropForeignKey("dbo.Courts", "ContactId", "dbo.Contacts");
            DropIndex("dbo.Courts", new[] { "ContactId" });
            DropColumn("dbo.Courts", "ContactId");
            DropTable("dbo.Contacts");
        }
    }
}

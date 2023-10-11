namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modify_Accounts_Add_ContactNo_and_BirthDate_fields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "ContactNo", c => c.String());
            AddColumn("dbo.Accounts", "BirthDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accounts", "BirthDate");
            DropColumn("dbo.Accounts", "ContactNo");
        }
    }
}

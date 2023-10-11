namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_Accounts_AddColumn_FirstName_and_LastName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "FirstName", c => c.String(maxLength: 30));
            AddColumn("dbo.Accounts", "LastName", c => c.String(maxLength: 30));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accounts", "LastName");
            DropColumn("dbo.Accounts", "FirstName");
        }
    }
}

namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modify_Accounts_Add_IsPublic : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "IsPublic", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accounts", "IsPublic");
        }
    }
}

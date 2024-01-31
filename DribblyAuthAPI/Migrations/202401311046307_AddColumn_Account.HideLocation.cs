namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumn_AccountHideLocation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "HideLocation", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accounts", "HideLocation");
        }
    }
}

namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modify_Accounts_Add_Status_Column : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accounts", "Status");
        }
    }
}

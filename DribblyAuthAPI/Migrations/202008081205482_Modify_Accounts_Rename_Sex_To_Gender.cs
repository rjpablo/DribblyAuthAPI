namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modify_Accounts_Rename_Sex_To_Gender : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "Gender", c => c.Int());
            DropColumn("dbo.Accounts", "Sex");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Accounts", "Sex", c => c.Int());
            DropColumn("dbo.Accounts", "Gender");
        }
    }
}

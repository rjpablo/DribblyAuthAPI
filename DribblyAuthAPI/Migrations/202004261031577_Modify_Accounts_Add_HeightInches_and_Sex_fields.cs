namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modify_Accounts_Add_HeightInches_and_Sex_fields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "HeightInches", c => c.Double());
            AddColumn("dbo.Accounts", "Sex", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accounts", "Sex");
            DropColumn("dbo.Accounts", "HeightInches");
        }
    }
}

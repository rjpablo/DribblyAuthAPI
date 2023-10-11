namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modify_Accounts_Make_Sex_and_BirthDate_nullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Accounts", "Sex", c => c.Int());
            AlterColumn("dbo.Accounts", "BirthDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Accounts", "BirthDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Accounts", "Sex", c => c.Int(nullable: false));
        }
    }
}

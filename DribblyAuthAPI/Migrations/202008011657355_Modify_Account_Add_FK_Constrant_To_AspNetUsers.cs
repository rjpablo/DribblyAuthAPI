namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modify_Account_Add_FK_Constrant_To_AspNetUsers : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Accounts", "IdentityUserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Accounts", "IdentityUserId");
            AddForeignKey("dbo.Accounts", "IdentityUserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Accounts", "IdentityUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Accounts", new[] { "IdentityUserId" });
            AlterColumn("dbo.Accounts", "IdentityUserId", c => c.String());
        }
    }
}

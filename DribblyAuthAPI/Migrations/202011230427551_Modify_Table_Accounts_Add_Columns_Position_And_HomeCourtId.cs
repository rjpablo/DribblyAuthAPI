namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modify_Table_Accounts_Add_Columns_Position_And_HomeCourtId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "Position", c => c.Int());
            AddColumn("dbo.Accounts", "HomeCourtId", c => c.Long());
            CreateIndex("dbo.Accounts", "HomeCourtId");
            AddForeignKey("dbo.Accounts", "HomeCourtId", "dbo.Courts", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Accounts", "HomeCourtId", "dbo.Courts");
            DropIndex("dbo.Accounts", new[] { "HomeCourtId" });
            DropColumn("dbo.Accounts", "HomeCourtId");
            DropColumn("dbo.Accounts", "Position");
        }
    }
}

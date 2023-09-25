namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumn_Games_TimekeeperId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "TimekeeperId", c => c.Long());
            CreateIndex("dbo.Games", "TimekeeperId");
            AddForeignKey("dbo.Games", "TimekeeperId", "dbo.Accounts", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Games", "TimekeeperId", "dbo.Accounts");
            DropIndex("dbo.Games", new[] { "TimekeeperId" });
            DropColumn("dbo.Games", "TimekeeperId");
        }
    }
}

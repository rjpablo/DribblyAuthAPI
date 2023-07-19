namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_GamePlayers_ModifyColumn_AccountId_MakeNonNullable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GamePlayers", "AccountId", "dbo.Accounts");
            DropIndex("dbo.GamePlayers", new[] { "AccountId" });
            AlterColumn("dbo.GamePlayers", "AccountId", c => c.Long(nullable: false));
            CreateIndex("dbo.GamePlayers", "AccountId");
            AddForeignKey("dbo.GamePlayers", "AccountId", "dbo.Accounts", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GamePlayers", "AccountId", "dbo.Accounts");
            DropIndex("dbo.GamePlayers", new[] { "AccountId" });
            AlterColumn("dbo.GamePlayers", "AccountId", c => c.Long());
            CreateIndex("dbo.GamePlayers", "AccountId");
            AddForeignKey("dbo.GamePlayers", "AccountId", "dbo.Accounts", "Id");
        }
    }
}

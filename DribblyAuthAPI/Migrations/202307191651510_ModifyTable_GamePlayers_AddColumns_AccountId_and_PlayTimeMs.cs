namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_GamePlayers_AddColumns_AccountId_and_PlayTimeMs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GamePlayers", "AccountId", c => c.Long());
            AddColumn("dbo.GamePlayers", "PlayTimeMs", c => c.Int(nullable: false));
            CreateIndex("dbo.GamePlayers", "AccountId");
            AddForeignKey("dbo.GamePlayers", "AccountId", "dbo.Accounts", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GamePlayers", "AccountId", "dbo.Accounts");
            DropIndex("dbo.GamePlayers", new[] { "AccountId" });
            DropColumn("dbo.GamePlayers", "PlayTimeMs");
            DropColumn("dbo.GamePlayers", "AccountId");
        }
    }
}

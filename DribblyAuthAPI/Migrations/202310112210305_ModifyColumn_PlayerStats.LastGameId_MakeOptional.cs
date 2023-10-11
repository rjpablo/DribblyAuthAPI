namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyColumn_PlayerStatsLastGameId_MakeOptional : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PlayerStats", "LastGameId", "dbo.Games");
            DropIndex("dbo.PlayerStats", new[] { "LastGameId" });
            AlterColumn("dbo.PlayerStats", "LastGameId", c => c.Long());
            CreateIndex("dbo.PlayerStats", "LastGameId");
            AddForeignKey("dbo.PlayerStats", "LastGameId", "dbo.Games", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PlayerStats", "LastGameId", "dbo.Games");
            DropIndex("dbo.PlayerStats", new[] { "LastGameId" });
            AlterColumn("dbo.PlayerStats", "LastGameId", c => c.Long(nullable: false));
            CreateIndex("dbo.PlayerStats", "LastGameId");
            AddForeignKey("dbo.PlayerStats", "LastGameId", "dbo.Games", "Id", cascadeDelete: true);
        }
    }
}

namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_Games_AddColumns_Team1Id_and_Team2Id : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "Team1Id", c => c.Long());
            AddColumn("dbo.Games", "Team2Id", c => c.Long());
            CreateIndex("dbo.Games", "Team1Id");
            CreateIndex("dbo.Games", "Team2Id");
            AddForeignKey("dbo.Games", "Team1Id", "dbo.GameTeams", "Id");
            AddForeignKey("dbo.Games", "Team2Id", "dbo.GameTeams", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Games", "Team2Id", "dbo.GameTeams");
            DropForeignKey("dbo.Games", "Team1Id", "dbo.GameTeams");
            DropIndex("dbo.Games", new[] { "Team2Id" });
            DropIndex("dbo.Games", new[] { "Team1Id" });
            DropColumn("dbo.Games", "Team2Id");
            DropColumn("dbo.Games", "Team1Id");
        }
    }
}

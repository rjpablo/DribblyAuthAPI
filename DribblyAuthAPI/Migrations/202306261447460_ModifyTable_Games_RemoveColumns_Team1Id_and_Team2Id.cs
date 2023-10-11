namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_Games_RemoveColumns_Team1Id_and_Team2Id : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Games", "Team1Id", "dbo.Teams");
            DropForeignKey("dbo.Games", "Team2Id", "dbo.Teams");
            DropIndex("dbo.Games", new[] { "Team1Id" });
            DropIndex("dbo.Games", new[] { "Team2Id" });
            DropColumn("dbo.Games", "Team1Id");
            DropColumn("dbo.Games", "Team2Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Games", "Team2Id", c => c.Long());
            AddColumn("dbo.Games", "Team1Id", c => c.Long());
            CreateIndex("dbo.Games", "Team2Id");
            CreateIndex("dbo.Games", "Team1Id");
            AddForeignKey("dbo.Games", "Team2Id", "dbo.Teams", "Id");
            AddForeignKey("dbo.Games", "Team1Id", "dbo.Teams", "Id");
        }
    }
}

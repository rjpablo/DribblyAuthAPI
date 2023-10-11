namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modify_Table_Games_Add_ForeignKey_For_Team1_And_Team2 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Games", "Team1Id");
            CreateIndex("dbo.Games", "Team2Id");
            AddForeignKey("dbo.Games", "Team1Id", "dbo.Teams", "Id");
            AddForeignKey("dbo.Games", "Team2Id", "dbo.Teams", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Games", "Team2Id", "dbo.Teams");
            DropForeignKey("dbo.Games", "Team1Id", "dbo.Teams");
            DropIndex("dbo.Games", new[] { "Team2Id" });
            DropIndex("dbo.Games", new[] { "Team1Id" });
        }
    }
}

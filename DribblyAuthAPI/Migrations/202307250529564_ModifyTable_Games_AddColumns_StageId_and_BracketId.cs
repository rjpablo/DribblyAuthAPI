namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_Games_AddColumns_StageId_and_BracketId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "StageId", c => c.Long());
            AddColumn("dbo.Games", "BracketId", c => c.Long());
            CreateIndex("dbo.Games", "StageId");
            CreateIndex("dbo.Games", "BracketId");
            AddForeignKey("dbo.Games", "BracketId", "dbo.StageBracketModels", "Id");
            AddForeignKey("dbo.Games", "StageId", "dbo.TournamentStages", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Games", "StageId", "dbo.TournamentStages");
            DropForeignKey("dbo.Games", "BracketId", "dbo.StageBracketModels");
            DropIndex("dbo.Games", new[] { "BracketId" });
            DropIndex("dbo.Games", new[] { "StageId" });
            DropColumn("dbo.Games", "BracketId");
            DropColumn("dbo.Games", "StageId");
        }
    }
}

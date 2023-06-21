namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_Games_AddColumnTournamentId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "TournamentId", c => c.Long());
            CreateIndex("dbo.Games", "TournamentId");
            AddForeignKey("dbo.Games", "TournamentId", "dbo.Tournaments", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Games", "TournamentId", "dbo.Tournaments");
            DropIndex("dbo.Games", new[] { "TournamentId" });
            DropColumn("dbo.Games", "TournamentId");
        }
    }
}

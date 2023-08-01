namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_TournamentTeams_ModifyKey_DropColumn_DateAdded : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.TournamentTeams");
            AddPrimaryKey("dbo.TournamentTeams", new[] { "TeamId", "TournamentId" });
            DropColumn("dbo.TournamentTeams", "Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TournamentTeams", "Id", c => c.Long(nullable: false));
            DropPrimaryKey("dbo.TournamentTeams");
            AddPrimaryKey("dbo.TournamentTeams", new[] { "Id", "TournamentId" });
        }
    }
}

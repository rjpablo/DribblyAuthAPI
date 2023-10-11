namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_TouranmentPlayers_AddColumn_JerseyNo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TournamentPlayers", "JerseyNo", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TournamentPlayers", "JerseyNo");
        }
    }
}

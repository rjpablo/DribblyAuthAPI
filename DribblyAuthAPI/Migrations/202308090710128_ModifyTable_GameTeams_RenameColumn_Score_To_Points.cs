namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_GameTeams_RenameColumn_Score_To_Points : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.GameTeams", "Score", "Points");
        }

        public override void Down()
        {
            RenameColumn("dbo.GameTeams", "Points", "Score");
        }
    }
}

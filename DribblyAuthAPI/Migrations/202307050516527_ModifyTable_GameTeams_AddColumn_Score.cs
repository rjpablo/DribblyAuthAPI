namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_GameTeams_AddColumn_Score : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GameTeams", "Score", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GameTeams", "Score");
        }
    }
}

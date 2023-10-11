namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_PlayerStats_AddColumn_PlayTimeMs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlayerStats", "PlayTimeMs", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PlayerStats", "PlayTimeMs");
        }
    }
}

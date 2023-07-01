namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_GameTeams_AddColumns_TimeoutsLeft_FullTimeoutsUsed_ShortTimeoutsUsed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GameTeams", "TimeoutsLeft", c => c.Int(nullable: false));
            AddColumn("dbo.GameTeams", "FullTimeoutsUsed", c => c.Int(nullable: false));
            AddColumn("dbo.GameTeams", "ShortTimeoutsUsed", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GameTeams", "ShortTimeoutsUsed");
            DropColumn("dbo.GameTeams", "FullTimeoutsUsed");
            DropColumn("dbo.GameTeams", "TimeoutsLeft");
        }
    }
}

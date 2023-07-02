namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_Games_AddColumn_IsInBonus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GameTeams", "IsInBonus", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GameTeams", "IsInBonus");
        }
    }
}

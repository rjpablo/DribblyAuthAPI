namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_Games_AddColumn_UsesRunningClock : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "UsesRunningClock", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Games", "UsesRunningClock");
        }
    }
}

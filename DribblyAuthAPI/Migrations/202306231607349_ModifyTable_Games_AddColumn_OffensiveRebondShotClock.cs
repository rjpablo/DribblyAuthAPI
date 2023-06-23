namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_Games_AddColumn_OffensiveRebondShotClock : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "OffensiveRebondShotClock", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Games", "OffensiveRebondShotClock");
        }
    }
}

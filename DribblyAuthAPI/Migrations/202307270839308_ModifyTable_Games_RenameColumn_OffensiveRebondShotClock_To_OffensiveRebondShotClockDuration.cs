namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_Games_RenameColumn_OffensiveRebondShotClock_To_OffensiveRebondShotClockDuration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "OffensiveRebondShotClockDuration", c => c.Int());
            DropColumn("dbo.Games", "OffensiveRebondShotClock");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Games", "OffensiveRebondShotClock", c => c.Int());
            DropColumn("dbo.Games", "OffensiveRebondShotClockDuration");
        }
    }
}

namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_Games_AddColumns_RemainingShotTime_and_DefaultShotClockDuration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "RemainingShotTime", c => c.Int());
            AddColumn("dbo.Games", "DefaultShotClockDuration", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Games", "DefaultShotClockDuration");
            DropColumn("dbo.Games", "RemainingShotTime");
        }
    }
}

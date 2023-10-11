namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_Tournaments_Add_columns_for_default_game_settings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tournaments", "TotalTimeoutLimit", c => c.Int(nullable: false));
            AddColumn("dbo.Tournaments", "FullTimeoutLimit", c => c.Int(nullable: false));
            AddColumn("dbo.Tournaments", "ShortTimeoutLimit", c => c.Int(nullable: false));
            AddColumn("dbo.Tournaments", "PersonalFoulLimit", c => c.Int(nullable: false));
            AddColumn("dbo.Tournaments", "TechnicalFoulLimit", c => c.Int(nullable: false));
            AddColumn("dbo.Tournaments", "IsTimed", c => c.Boolean(nullable: false));
            AddColumn("dbo.Tournaments", "UsesRunningClock", c => c.Boolean(nullable: false));
            AddColumn("dbo.Tournaments", "OvertimePeriodDuration", c => c.Int(nullable: false));
            AddColumn("dbo.Tournaments", "DefaultShotClockDuration", c => c.Int());
            AddColumn("dbo.Tournaments", "OffensiveRebondShotClockDuration", c => c.Int());
            AddColumn("dbo.Tournaments", "NumberOfRegulationPeriods", c => c.Int(nullable: false));
            AddColumn("dbo.Tournaments", "RegulationPeriodDuration", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tournaments", "RegulationPeriodDuration");
            DropColumn("dbo.Tournaments", "NumberOfRegulationPeriods");
            DropColumn("dbo.Tournaments", "OffensiveRebondShotClockDuration");
            DropColumn("dbo.Tournaments", "DefaultShotClockDuration");
            DropColumn("dbo.Tournaments", "OvertimePeriodDuration");
            DropColumn("dbo.Tournaments", "UsesRunningClock");
            DropColumn("dbo.Tournaments", "IsTimed");
            DropColumn("dbo.Tournaments", "TechnicalFoulLimit");
            DropColumn("dbo.Tournaments", "PersonalFoulLimit");
            DropColumn("dbo.Tournaments", "ShortTimeoutLimit");
            DropColumn("dbo.Tournaments", "FullTimeoutLimit");
            DropColumn("dbo.Tournaments", "TotalTimeoutLimit");
        }
    }
}

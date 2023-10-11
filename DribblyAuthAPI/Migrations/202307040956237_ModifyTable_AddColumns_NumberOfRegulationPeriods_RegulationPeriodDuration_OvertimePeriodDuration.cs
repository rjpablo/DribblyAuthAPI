namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_AddColumns_NumberOfRegulationPeriods_RegulationPeriodDuration_OvertimePeriodDuration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "RegulationPeriodDuration", c => c.Int(nullable: false));
            AddColumn("dbo.Games", "NumberOfRegulationPeriods", c => c.Int(nullable: false));
            AddColumn("dbo.Games", "OvertimePeriodDuration", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Games", "OvertimePeriodDuration");
            DropColumn("dbo.Games", "NumberOfRegulationPeriods");
            DropColumn("dbo.Games", "RegulationPeriodDuration");
        }
    }
}

namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_Games_AddColumns_CurrentPeriod_RemainingTime_and_IsTimed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "CurrentPeriod", c => c.Int(nullable: false));
            AddColumn("dbo.Games", "RemainingTime", c => c.Int(nullable: false));
            AddColumn("dbo.Games", "IsTimed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Games", "IsTimed");
            DropColumn("dbo.Games", "RemainingTime");
            DropColumn("dbo.Games", "CurrentPeriod");
        }
    }
}

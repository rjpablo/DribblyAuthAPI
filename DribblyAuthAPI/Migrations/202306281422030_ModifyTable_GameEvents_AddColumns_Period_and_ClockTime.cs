namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_GameEvents_AddColumns_Period_and_ClockTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GameEvents", "Period", c => c.Int());
            AddColumn("dbo.GameEvents", "ClockTime", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GameEvents", "ClockTime");
            DropColumn("dbo.GameEvents", "Period");
        }
    }
}

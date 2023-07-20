namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_PlayerStats_AddColumn_OverallScore : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlayerStats", "OverallScore", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PlayerStats", "OverallScore");
        }
    }
}

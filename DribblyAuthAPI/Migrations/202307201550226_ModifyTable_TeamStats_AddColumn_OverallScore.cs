namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_TeamStats_AddColumn_OverallScore : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TeamStats", "OverallScore", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TeamStats", "OverallScore");
        }
    }
}

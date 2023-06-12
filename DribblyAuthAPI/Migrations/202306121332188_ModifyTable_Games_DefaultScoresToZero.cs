namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_Games_DefaultScoresToZero : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Games", "Team1Score", c => c.Int(nullable: false, defaultValue: 0));
            AlterColumn("dbo.Games", "Team2Score", c => c.Int(nullable: false, defaultValue: 0));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Games", "Team2Score", c => c.Int());
            AlterColumn("dbo.Games", "Team1Score", c => c.Int());
        }
    }
}

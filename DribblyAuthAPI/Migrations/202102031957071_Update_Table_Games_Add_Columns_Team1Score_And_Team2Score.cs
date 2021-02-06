namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_Table_Games_Add_Columns_Team1Score_And_Team2Score : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "Team1Score", c => c.Int());
            AddColumn("dbo.Games", "Team2Score", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Games", "Team2Score");
            DropColumn("dbo.Games", "Team1Score");
        }
    }
}

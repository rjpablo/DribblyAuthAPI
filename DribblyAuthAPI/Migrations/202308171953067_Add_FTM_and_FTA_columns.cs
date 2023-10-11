namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_FTM_and_FTA_columns : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GameTeams", "FTA", c => c.Int(nullable: false));
            AddColumn("dbo.GameTeams", "FTM", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GameTeams", "FTM");
            DropColumn("dbo.GameTeams", "FTA");
        }
    }
}

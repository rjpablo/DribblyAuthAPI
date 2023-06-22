namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_Games_AddColumn_RemainingTimeUpdatedAt : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "RemainingTimeUpdatedAt", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Games", "RemainingTimeUpdatedAt");
        }
    }
}

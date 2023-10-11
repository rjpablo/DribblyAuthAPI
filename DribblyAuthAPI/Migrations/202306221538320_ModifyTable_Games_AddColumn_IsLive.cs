namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_Games_AddColumn_IsLive : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "IsLive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Games", "IsLive");
        }
    }
}

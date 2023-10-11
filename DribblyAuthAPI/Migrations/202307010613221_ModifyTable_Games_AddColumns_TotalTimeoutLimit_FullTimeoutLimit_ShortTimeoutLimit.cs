namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_Games_AddColumns_TotalTimeoutLimit_FullTimeoutLimit_ShortTimeoutLimit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "TotalTimeoutLimit", c => c.Int(nullable: false));
            AddColumn("dbo.Games", "FullTimeoutLimit", c => c.Int(nullable: false));
            AddColumn("dbo.Games", "ShortTimeoutLimit", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Games", "ShortTimeoutLimit");
            DropColumn("dbo.Games", "FullTimeoutLimit");
            DropColumn("dbo.Games", "TotalTimeoutLimit");
        }
    }
}

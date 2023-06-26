namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_Games_AddColumn_PersonalFoulLimit_and_TechnicalFoulLimit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "PersonalFoulLimit", c => c.Int(nullable: false));
            AddColumn("dbo.Games", "TechnicalFoulLimit", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Games", "TechnicalFoulLimit");
            DropColumn("dbo.Games", "PersonalFoulLimit");
        }
    }
}

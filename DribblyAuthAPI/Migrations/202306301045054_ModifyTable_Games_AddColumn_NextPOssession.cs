namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_Games_AddColumn_NextPOssession : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "NextPossession", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Games", "NextPossession");
        }
    }
}

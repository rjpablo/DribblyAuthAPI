namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumn_RequireApproval_ToTable_Events : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "RequireApproval", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "RequireApproval");
        }
    }
}

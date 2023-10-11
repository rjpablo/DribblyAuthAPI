namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_TeamMemberships_AddColumn_JerseyNo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TeamMemberships", "JerseyNo", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TeamMemberships", "JerseyNo");
        }
    }
}

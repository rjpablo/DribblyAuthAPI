namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_JoinTeamRequests_AddColumn_JerseyNo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.JoinTeamRequests", "JerseyNo", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.JoinTeamRequests", "JerseyNo");
        }
    }
}

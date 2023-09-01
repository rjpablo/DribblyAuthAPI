namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveColumns_TeamMembershipPosition_and_JoinTeamRequestPosition : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.TeamMemberships", "Position");
            DropColumn("dbo.JoinTeamRequests", "Position");
        }
        
        public override void Down()
        {
            AddColumn("dbo.JoinTeamRequests", "Position", c => c.Int(nullable: false));
            AddColumn("dbo.TeamMemberships", "Position", c => c.Int(nullable: false));
        }
    }
}

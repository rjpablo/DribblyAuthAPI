namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_TeamMemberShips_Add_StatsColumns : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TeamMemberships", "GP", c => c.Int(nullable: false));
            AddColumn("dbo.TeamMemberships", "GW", c => c.Int(nullable: false));
            AddColumn("dbo.TeamMemberships", "PPG", c => c.Double(nullable: false));
            AddColumn("dbo.TeamMemberships", "RPG", c => c.Double(nullable: false));
            AddColumn("dbo.TeamMemberships", "APG", c => c.Double(nullable: false));
            AddColumn("dbo.TeamMemberships", "FGP", c => c.Double(nullable: false));
            AddColumn("dbo.TeamMemberships", "BPG", c => c.Double(nullable: false));
            AddColumn("dbo.TeamMemberships", "TPG", c => c.Int(nullable: false));
            AddColumn("dbo.TeamMemberships", "SPG", c => c.Int(nullable: false));
            AddColumn("dbo.TeamMemberships", "ThreePP", c => c.Double(nullable: false));
            AddColumn("dbo.TeamMemberships", "OverallScore", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TeamMemberships", "OverallScore");
            DropColumn("dbo.TeamMemberships", "ThreePP");
            DropColumn("dbo.TeamMemberships", "SPG");
            DropColumn("dbo.TeamMemberships", "TPG");
            DropColumn("dbo.TeamMemberships", "BPG");
            DropColumn("dbo.TeamMemberships", "FGP");
            DropColumn("dbo.TeamMemberships", "APG");
            DropColumn("dbo.TeamMemberships", "RPG");
            DropColumn("dbo.TeamMemberships", "PPG");
            DropColumn("dbo.TeamMemberships", "GW");
            DropColumn("dbo.TeamMemberships", "GP");
        }
    }
}

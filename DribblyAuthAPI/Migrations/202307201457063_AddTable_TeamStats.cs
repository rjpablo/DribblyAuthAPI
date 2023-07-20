namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTable_TeamStats : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TeamStats",
                c => new
                    {
                        TeamId = c.Long(nullable: false),
                        GP = c.Int(nullable: false),
                        GW = c.Int(nullable: false),
                        PPG = c.Double(nullable: false),
                        RPG = c.Double(nullable: false),
                        APG = c.Double(nullable: false),
                        FGP = c.Double(nullable: false),
                        BPG = c.Double(nullable: false),
                        ThreePP = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.TeamId)
                .ForeignKey("dbo.Teams", t => t.TeamId)
                .Index(t => t.TeamId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TeamStats", "TeamId", "dbo.Teams");
            DropIndex("dbo.TeamStats", new[] { "TeamId" });
            DropTable("dbo.TeamStats");
        }
    }
}

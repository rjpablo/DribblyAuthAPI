namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTable_PlayerStats : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PlayerStats",
                c => new
                    {
                        AccountId = c.Long(nullable: false),
                        GP = c.Int(nullable: false),
                        GW = c.Int(nullable: false),
                        PPG = c.Double(nullable: false),
                        RPG = c.Double(nullable: false),
                        APG = c.Double(nullable: false),
                        FGP = c.Double(nullable: false),
                        ThreePP = c.Double(nullable: false),
                        FTP = c.Double(nullable: false),
                        BPG = c.Double(nullable: false),
                        MPG = c.Double(nullable: false),
                        LastGameId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.AccountId)
                .ForeignKey("dbo.Accounts", t => t.AccountId)
                .ForeignKey("dbo.Games", t => t.LastGameId, cascadeDelete: true)
                .Index(t => t.AccountId)
                .Index(t => t.LastGameId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PlayerStats", "LastGameId", "dbo.Games");
            DropForeignKey("dbo.PlayerStats", "AccountId", "dbo.Accounts");
            DropIndex("dbo.PlayerStats", new[] { "LastGameId" });
            DropIndex("dbo.PlayerStats", new[] { "AccountId" });
            DropTable("dbo.PlayerStats");
        }
    }
}

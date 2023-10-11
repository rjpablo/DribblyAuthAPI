namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTable_Seasons : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Seasons",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AddedById = c.Long(nullable: false),
                        Name = c.String(),
                        LeagueId = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Leagues", t => t.LeagueId, cascadeDelete: true)
                .Index(t => t.LeagueId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Seasons", "LeagueId", "dbo.Leagues");
            DropIndex("dbo.Seasons", new[] { "LeagueId" });
            DropTable("dbo.Seasons");
        }
    }
}

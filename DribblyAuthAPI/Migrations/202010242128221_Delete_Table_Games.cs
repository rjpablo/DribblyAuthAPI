namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Delete_Table_Games : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Games", "Id", "dbo.Bookings");
            DropIndex("dbo.Games", new[] { "Id" });
            DropTable("dbo.Games");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Games",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        Team1Id = c.Long(nullable: false),
                        Team2Id = c.Long(nullable: false),
                        WinningTeamId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.Games", "Id");
            AddForeignKey("dbo.Games", "Id", "dbo.Bookings", "Id");
        }
    }
}

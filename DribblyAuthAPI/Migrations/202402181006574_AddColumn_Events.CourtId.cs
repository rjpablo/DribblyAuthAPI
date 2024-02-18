namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumn_EventsCourtId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "CourtId", c => c.Long());
            CreateIndex("dbo.Events", "CourtId");
            AddForeignKey("dbo.Events", "CourtId", "dbo.Courts", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Events", "CourtId", "dbo.Courts");
            DropIndex("dbo.Events", new[] { "CourtId" });
            DropColumn("dbo.Events", "CourtId");
        }
    }
}

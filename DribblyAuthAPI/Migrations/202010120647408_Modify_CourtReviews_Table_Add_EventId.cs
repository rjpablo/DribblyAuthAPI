namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modify_CourtReviews_Table_Add_EventId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CourtReviews", "EventId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CourtReviews", "EventId");
        }
    }
}

namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modify_CourtModel_Added_DateDeleted_Column : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Videos", "DateDeleted", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Videos", "DateDeleted");
        }
    }
}

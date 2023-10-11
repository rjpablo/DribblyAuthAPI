namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modify_Events_Add_Columns_HasReviewed_And_Status : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "HasReviewed", c => c.Boolean(nullable: false));
            AddColumn("dbo.Events", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "Status");
            DropColumn("dbo.Events", "HasReviewed");
        }
    }
}

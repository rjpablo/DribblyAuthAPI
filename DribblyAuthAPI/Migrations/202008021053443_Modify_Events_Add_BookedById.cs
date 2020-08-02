namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modify_Events_Add_BookedById : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "BookedById", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "BookedById");
        }
    }
}

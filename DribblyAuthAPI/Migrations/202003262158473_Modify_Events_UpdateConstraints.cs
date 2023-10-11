namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modify_Events_UpdateConstraints : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Events", "AddedBy", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Events", "AddedBy", c => c.String());
        }
    }
}

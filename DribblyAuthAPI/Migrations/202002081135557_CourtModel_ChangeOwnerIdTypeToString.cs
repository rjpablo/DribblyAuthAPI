namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CourtModel_ChangeOwnerIdTypeToString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Courts", "OwnerId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Courts", "OwnerId", c => c.Long(nullable: false));
        }
    }
}

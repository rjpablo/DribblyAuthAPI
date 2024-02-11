namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumns_VisitCount_and_LastModified_toTable_BlogModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Blogs", "VisitCount", c => c.Int(nullable: false));
            AddColumn("dbo.Blogs", "LastModified", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Blogs", "LastModified");
            DropColumn("dbo.Blogs", "VisitCount");
        }
    }
}

namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_PhotoModel_DateDeletedField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Photos", "DateDeleted", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Photos", "DateDeleted");
        }
    }
}

namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumn_Order_ToTable_FeaturedEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FeaturedEntities", "Order", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FeaturedEntities", "Order");
        }
    }
}

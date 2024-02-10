namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTable_FeaturedEntities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FeaturedEntities",
                c => new
                    {
                        EntityId = c.Long(nullable: false),
                        EntityType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.EntityId, t.EntityType });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FeaturedEntities");
        }
    }
}

namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_CourtReviews_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CourtReviews",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CourtId = c.Long(nullable: false),
                        Rating = c.Double(nullable: false),
                        Message = c.String(),
                        ReviewedById = c.String(),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Courts", t => t.CourtId, cascadeDelete: true)
                .Index(t => t.CourtId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CourtReviews", "CourtId", "dbo.Courts");
            DropIndex("dbo.CourtReviews", new[] { "CourtId" });
            DropTable("dbo.CourtReviews");
        }
    }
}

namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_CourtFollowings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CourtFollowings",
                c => new
                    {
                        CourtId = c.Long(nullable: false),
                        FollowedById = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.CourtId, t.FollowedById });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CourtFollowings");
        }
    }
}

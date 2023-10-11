namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcourtmodel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Courts",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        OwnerId = c.Long(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        RatePerHour = c.Decimal(precision: 18, scale: 2),
                        Rating = c.Decimal(precision: 18, scale: 2),
                        DateAdded = c.DateTime(nullable: false),
                        PrimaryPhotoUrl = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Courts");
        }
    }
}

namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Accounts_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        IdentityUserId = c.String(),
                        ProfilePhotoId = c.Long(),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Photos", t => t.ProfilePhotoId)
                .Index(t => t.ProfilePhotoId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Accounts", "ProfilePhotoId", "dbo.Photos");
            DropIndex("dbo.Accounts", new[] { "ProfilePhotoId" });
            DropTable("dbo.Accounts");
        }
    }
}

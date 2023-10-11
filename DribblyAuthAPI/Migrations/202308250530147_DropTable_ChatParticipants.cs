namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropTable_ChatParticipants : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Participants", "PhotoId", "dbo.Multimedia");
            DropIndex("dbo.Participants", new[] { "PhotoId" });
            DropTable("dbo.Participants");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Participants",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        PhotoId = c.Long(),
                        AccountId = c.Long(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.Participants", "PhotoId");
            AddForeignKey("dbo.Participants", "PhotoId", "dbo.Multimedia", "Id");
        }
    }
}

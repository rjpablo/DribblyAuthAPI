namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropTable_Multimedia : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Chats", "IconId", "dbo.Multimedia");
            DropForeignKey("dbo.MessageMedia", "MediaId", "dbo.Multimedia");
            DropForeignKey("dbo.ChatParticipants", "PhotoId", "dbo.Multimedia");
            DropIndex("dbo.ChatParticipants", new[] { "PhotoId" });
            DropIndex("dbo.Chats", new[] { "IconId" });
            DropIndex("dbo.MessageMedia", new[] { "MediaId" });
            DropTable("dbo.Multimedia");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Multimedia",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Url = c.String(),
                        Type = c.Int(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.MessageMedia", "MediaId");
            CreateIndex("dbo.Chats", "IconId");
            CreateIndex("dbo.ChatParticipants", "PhotoId");
            AddForeignKey("dbo.ChatParticipants", "PhotoId", "dbo.Multimedia", "Id");
            AddForeignKey("dbo.MessageMedia", "MediaId", "dbo.Multimedia", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Chats", "IconId", "dbo.Multimedia", "Id");
        }
    }
}

namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Link_Chat_Tables_To_PhotoModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Photos", "Type", c => c.Int(nullable: false));
            CreateIndex("dbo.ChatParticipants", "PhotoId");
            CreateIndex("dbo.Chats", "IconId");
            CreateIndex("dbo.MessageMedia", "MediaId");
            AddForeignKey("dbo.Chats", "IconId", "dbo.Photos", "Id");
            AddForeignKey("dbo.MessageMedia", "MediaId", "dbo.Photos", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ChatParticipants", "PhotoId", "dbo.Photos", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChatParticipants", "PhotoId", "dbo.Photos");
            DropForeignKey("dbo.MessageMedia", "MediaId", "dbo.Photos");
            DropForeignKey("dbo.Chats", "IconId", "dbo.Photos");
            DropIndex("dbo.MessageMedia", new[] { "MediaId" });
            DropIndex("dbo.Chats", new[] { "IconId" });
            DropIndex("dbo.ChatParticipants", new[] { "PhotoId" });
            DropColumn("dbo.Photos", "Type");
        }
    }
}

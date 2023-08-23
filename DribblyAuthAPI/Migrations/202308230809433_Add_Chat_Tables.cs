namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Chat_Tables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChatParticipants",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        ChatId = c.Long(nullable: false),
                        ParticipantId = c.Long(nullable: false),
                        PhotoId = c.Long(),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Chats", t => t.ChatId, cascadeDelete: true)
                .ForeignKey("dbo.Multimedia", t => t.PhotoId)
                .Index(t => t.ChatId)
                .Index(t => t.PhotoId);
            
            CreateTable(
                "dbo.Chats",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        LastUpdateTime = c.DateTime(nullable: false),
                        Title = c.String(),
                        Type = c.Int(nullable: false),
                        IsTemporary = c.Boolean(nullable: false),
                        IconId = c.Long(),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Multimedia", t => t.IconId)
                .Index(t => t.IconId);
            
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
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        SenderId = c.Long(nullable: false),
                        Text = c.String(),
                        ChatId = c.Long(),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Chats", t => t.ChatId)
                .Index(t => t.ChatId);
            
            CreateTable(
                "dbo.MessageMedia",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        MessageId = c.Long(nullable: false),
                        MediaId = c.Long(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Multimedia", t => t.MediaId, cascadeDelete: true)
                .ForeignKey("dbo.Messages", t => t.MessageId, cascadeDelete: true)
                .Index(t => t.MessageId)
                .Index(t => t.MediaId);
            
            CreateTable(
                "dbo.ParticipantMessages",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        MessageId = c.Long(nullable: false),
                        ParticipantId = c.Long(nullable: false),
                        IsSender = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Messages", t => t.MessageId, cascadeDelete: true)
                .Index(t => t.MessageId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChatParticipants", "PhotoId", "dbo.Multimedia");
            DropForeignKey("dbo.ChatParticipants", "ChatId", "dbo.Chats");
            DropForeignKey("dbo.ParticipantMessages", "MessageId", "dbo.Messages");
            DropForeignKey("dbo.MessageMedia", "MessageId", "dbo.Messages");
            DropForeignKey("dbo.MessageMedia", "MediaId", "dbo.Multimedia");
            DropForeignKey("dbo.Messages", "ChatId", "dbo.Chats");
            DropForeignKey("dbo.Chats", "IconId", "dbo.Multimedia");
            DropIndex("dbo.ParticipantMessages", new[] { "MessageId" });
            DropIndex("dbo.MessageMedia", new[] { "MediaId" });
            DropIndex("dbo.MessageMedia", new[] { "MessageId" });
            DropIndex("dbo.Messages", new[] { "ChatId" });
            DropIndex("dbo.Chats", new[] { "IconId" });
            DropIndex("dbo.ChatParticipants", new[] { "PhotoId" });
            DropIndex("dbo.ChatParticipants", new[] { "ChatId" });
            DropTable("dbo.ParticipantMessages");
            DropTable("dbo.MessageMedia");
            DropTable("dbo.Messages");
            DropTable("dbo.Multimedia");
            DropTable("dbo.Chats");
            DropTable("dbo.ChatParticipants");
        }
    }
}

namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Use_AccountModel_as_ChatParticipant_and_MessageParticipant : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Accounts", "HomeCourtId", "dbo.Courts");
            DropForeignKey("dbo.Accounts", "IdentityUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ChatParticipants", "PhotoId", "dbo.Multimedia");
            DropForeignKey("dbo.ChatParticipants", "PhotoId", "dbo.Photos");
            DropForeignKey("dbo.UserAccountActivities", "AccountId", "dbo.Accounts");
            DropForeignKey("dbo.AccountPhotos", "AccountId", "dbo.Accounts");
            DropForeignKey("dbo.AccountVideos", "AccountId", "dbo.Accounts");
            DropForeignKey("dbo.StageBracketModels", "AddedById", "dbo.Accounts");
            DropForeignKey("dbo.TournamentStages", "AddedById", "dbo.Accounts");
            DropForeignKey("dbo.TeamMemberships", "MemberAccountId", "dbo.Accounts");
            DropForeignKey("dbo.JoinTournamentRequests", "AddedByID", "dbo.Accounts");
            DropForeignKey("dbo.GamePlayers", "AccountId", "dbo.Accounts");
            DropForeignKey("dbo.JoinTeamRequests", "MemberAccountId", "dbo.Accounts");
            DropForeignKey("dbo.NewBookingNotifications", "BookedById", "dbo.Accounts");
            DropForeignKey("dbo.NewGameNotifications", "BookedById", "dbo.Accounts");
            DropForeignKey("dbo.UpdateGameNotifications", "UpdatedById", "dbo.Accounts");
            DropForeignKey("dbo.Posts", "AddedById", "dbo.Accounts");
            DropForeignKey("dbo.TournamentPlayers", "AccountId", "dbo.Accounts");
            DropIndex("dbo.Accounts", new[] { "IdentityUserId" });
            DropIndex("dbo.Accounts", new[] { "HomeCourtId" });
            DropIndex("dbo.ChatParticipants", new[] { "PhotoId" });
            DropPrimaryKey("dbo.ChatParticipants");
            DropPrimaryKey("dbo.ParticipantMessages");
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Multimedia", t => t.PhotoId)
                .Index(t => t.PhotoId);
            
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        IdentityUserId = c.Long(nullable: false),
                        Position = c.Int(),
                        HomeCourtId = c.Long(),
                        HeightInches = c.Double(),
                        Rating = c.Double(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.IdentityUserId, cascadeDelete: true)
                .ForeignKey("dbo.Courts", t => t.HomeCourtId)
                .Index(t => t.Id)
                .Index(t => t.IdentityUserId)
                .Index(t => t.HomeCourtId);
            
            AddColumn("dbo.Chats", "Code", c => c.String());
            AddPrimaryKey("dbo.ChatParticipants", new[] { "ChatId", "ParticipantId" });
            AddPrimaryKey("dbo.ParticipantMessages", new[] { "MessageId", "ParticipantId" });
            CreateIndex("dbo.ParticipantMessages", "ParticipantId");
            CreateIndex("dbo.ChatParticipants", "ParticipantId");
            AddForeignKey("dbo.ParticipantMessages", "ParticipantId", "dbo.Accounts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ChatParticipants", "ParticipantId", "dbo.Accounts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.UserAccountActivities", "AccountId", "dbo.Players", "Id");
            AddForeignKey("dbo.AccountPhotos", "AccountId", "dbo.Players", "Id");
            AddForeignKey("dbo.AccountVideos", "AccountId", "dbo.Players", "Id");
            AddForeignKey("dbo.StageBracketModels", "AddedById", "dbo.Players", "Id");
            AddForeignKey("dbo.TournamentStages", "AddedById", "dbo.Players", "Id");
            AddForeignKey("dbo.TeamMemberships", "MemberAccountId", "dbo.Players", "Id");
            AddForeignKey("dbo.JoinTournamentRequests", "AddedByID", "dbo.Players", "Id");
            AddForeignKey("dbo.GamePlayers", "AccountId", "dbo.Players", "Id");
            AddForeignKey("dbo.JoinTeamRequests", "MemberAccountId", "dbo.Players", "Id");
            AddForeignKey("dbo.NewBookingNotifications", "BookedById", "dbo.Players", "Id");
            AddForeignKey("dbo.NewGameNotifications", "BookedById", "dbo.Players", "Id");
            AddForeignKey("dbo.UpdateGameNotifications", "UpdatedById", "dbo.Players", "Id");
            AddForeignKey("dbo.Posts", "AddedById", "dbo.Players", "Id");
            AddForeignKey("dbo.TournamentPlayers", "AccountId", "dbo.Players", "Id");
            DropColumn("dbo.Accounts", "IdentityUserId");
            DropColumn("dbo.Accounts", "Position");
            DropColumn("dbo.Accounts", "HomeCourtId");
            DropColumn("dbo.Accounts", "HeightInches");
            DropColumn("dbo.Accounts", "Rating");
            DropColumn("dbo.ChatParticipants", "Id");
            DropColumn("dbo.ChatParticipants", "Name");
            DropColumn("dbo.ChatParticipants", "PhotoId");
            DropColumn("dbo.ParticipantMessages", "Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ParticipantMessages", "Id", c => c.Long(nullable: false, identity: true));
            AddColumn("dbo.ChatParticipants", "PhotoId", c => c.Long());
            AddColumn("dbo.ChatParticipants", "Name", c => c.String());
            AddColumn("dbo.ChatParticipants", "Id", c => c.Long(nullable: false, identity: true));
            AddColumn("dbo.Accounts", "Rating", c => c.Double());
            AddColumn("dbo.Accounts", "HeightInches", c => c.Double());
            AddColumn("dbo.Accounts", "HomeCourtId", c => c.Long());
            AddColumn("dbo.Accounts", "Position", c => c.Int());
            AddColumn("dbo.Accounts", "IdentityUserId", c => c.Long(nullable: false));
            DropForeignKey("dbo.TournamentPlayers", "AccountId", "dbo.Players");
            DropForeignKey("dbo.Posts", "AddedById", "dbo.Players");
            DropForeignKey("dbo.UpdateGameNotifications", "UpdatedById", "dbo.Players");
            DropForeignKey("dbo.NewGameNotifications", "BookedById", "dbo.Players");
            DropForeignKey("dbo.NewBookingNotifications", "BookedById", "dbo.Players");
            DropForeignKey("dbo.JoinTeamRequests", "MemberAccountId", "dbo.Players");
            DropForeignKey("dbo.GamePlayers", "AccountId", "dbo.Players");
            DropForeignKey("dbo.JoinTournamentRequests", "AddedByID", "dbo.Players");
            DropForeignKey("dbo.TeamMemberships", "MemberAccountId", "dbo.Players");
            DropForeignKey("dbo.TournamentStages", "AddedById", "dbo.Players");
            DropForeignKey("dbo.StageBracketModels", "AddedById", "dbo.Players");
            DropForeignKey("dbo.AccountVideos", "AccountId", "dbo.Players");
            DropForeignKey("dbo.AccountPhotos", "AccountId", "dbo.Players");
            DropForeignKey("dbo.UserAccountActivities", "AccountId", "dbo.Players");
            DropForeignKey("dbo.Players", "HomeCourtId", "dbo.Courts");
            DropForeignKey("dbo.Players", "IdentityUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Players", "Id", "dbo.Accounts");
            DropForeignKey("dbo.ChatParticipants", "ParticipantId", "dbo.Accounts");
            DropForeignKey("dbo.ParticipantMessages", "ParticipantId", "dbo.Accounts");
            DropForeignKey("dbo.Participants", "PhotoId", "dbo.Multimedia");
            DropIndex("dbo.Players", new[] { "HomeCourtId" });
            DropIndex("dbo.Players", new[] { "IdentityUserId" });
            DropIndex("dbo.Players", new[] { "Id" });
            DropIndex("dbo.ChatParticipants", new[] { "ParticipantId" });
            DropIndex("dbo.ParticipantMessages", new[] { "ParticipantId" });
            DropIndex("dbo.Participants", new[] { "PhotoId" });
            DropPrimaryKey("dbo.ParticipantMessages");
            DropPrimaryKey("dbo.ChatParticipants");
            DropColumn("dbo.Chats", "Code");
            DropTable("dbo.Players");
            DropTable("dbo.Participants");
            AddPrimaryKey("dbo.ParticipantMessages", "Id");
            AddPrimaryKey("dbo.ChatParticipants", "Id");
            CreateIndex("dbo.ChatParticipants", "PhotoId");
            CreateIndex("dbo.Accounts", "HomeCourtId");
            CreateIndex("dbo.Accounts", "IdentityUserId");
            AddForeignKey("dbo.TournamentPlayers", "AccountId", "dbo.Accounts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Posts", "AddedById", "dbo.Accounts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.UpdateGameNotifications", "UpdatedById", "dbo.Accounts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.NewGameNotifications", "BookedById", "dbo.Accounts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.NewBookingNotifications", "BookedById", "dbo.Accounts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.JoinTeamRequests", "MemberAccountId", "dbo.Accounts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.GamePlayers", "AccountId", "dbo.Accounts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.JoinTournamentRequests", "AddedByID", "dbo.Accounts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TeamMemberships", "MemberAccountId", "dbo.Accounts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TournamentStages", "AddedById", "dbo.Accounts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.StageBracketModels", "AddedById", "dbo.Accounts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AccountVideos", "AccountId", "dbo.Accounts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AccountPhotos", "AccountId", "dbo.Accounts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.UserAccountActivities", "AccountId", "dbo.Accounts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ChatParticipants", "PhotoId", "dbo.Multimedia", "Id");
            AddForeignKey("dbo.Accounts", "IdentityUserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Accounts", "HomeCourtId", "dbo.Courts", "Id");
        }
    }
}

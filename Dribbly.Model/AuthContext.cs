﻿using Dribbly.Authentication.Models;
using Dribbly.Chat.Data;
using Dribbly.Chat.Models;
using Dribbly.Core.Models;
using Dribbly.Identity.Models;
using Dribbly.Model.Account;
using Dribbly.Model.Accounts;
using Dribbly.Model.Bookings;
using Dribbly.Model.Courts;
using Dribbly.Model.Entities;
using Dribbly.Model.Entities.Groups;
using Dribbly.Model.Fouls;
using Dribbly.Model.Games;
using Dribbly.Model.Leagues;
using Dribbly.Model.Logs;
using Dribbly.Model.Notifications;
using Dribbly.Model.Posts;
using Dribbly.Model.Seasons;
using Dribbly.Model.Shared;
using Dribbly.Model.Teams;
using Dribbly.Model.Tournaments;
using Dribbly.Model.UserActivities;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Dribbly.Model
{
    public interface IAuthContext : IDisposable, IChatDbContext
    {
        DbSet<Client> Clients { get; set; }
        DbSet<RefreshToken> RefreshTokens { get; set; }
        DbSet<LeagueModel> Leagues { get; set; }
        DbSet<SeasonModel> Seasons { get; set; }

        #region Tournaments
        DbSet<TournamentModel> Tournaments { get; set; }
        DbSet<TournamentTeamModel> TournamentTeams { get; set; }
        DbSet<TournamentStageModel> TournamentStages { get; set; }
        DbSet<StageTeamModel> StageTeams { get; set; }
        DbSet<StageBracketModel> StageBrackets { get; set; }
        DbSet<TournamentPhotoModel> TournamentPhotos { get; set; }
        DbSet<TournamentPlayerModel> TournamentPlayers { get; set; }
        #endregion

        DbSet<JoinTournamentRequestModel> JoinTournamentRequests { get; set; }
        DbSet<CourtModel> Courts { get; set; }
        DbSet<CourtFollowingModel> CourtFollowings { get; set; }
        DbSet<SettingModel> Settings { get; set; }
        DbSet<CourtPhotoModel> CourtPhotos { get; set; }
        DbSet<BookingModel> Bookings { get; set; }
        DbSet<GameModel> Games { get; set; }
        DbSet<GameTeamModel> GameTeams { get; set; }
        DbSet<GamePlayerModel> GamePlayers { get; set; }
        DbSet<PlayerStatsModel> PlayerStats { get; set; }
        DbSet<FoulModel> Fouls { get; set; }
        DbSet<MemberFoulModel> MemberFouls { get; set; }
        DbSet<GameEventModel> GameEvents { get; set; }
        DbSet<ShotModel> Shots { get; set; }
        DbSet<TeamModel> Teams { get; set; }
        DbSet<TeamStatsModel> TeamStats { get; set; }
        DbSet<TeamMembershipModel> TeamMembers { get; set; }
        DbSet<JoinTeamRequestModel> JoinTeamRequests { get; set; }
        DbSet<UserPermissionModel> UserPermissions { get; set; }
        DbSet<AccountPhotoModel> AccountPhotos { get; set; }
        DbSet<AccountHighlightModel> AccountHighlights { get; set; }
        DbSet<TeamPhotoModel> TeamPhotos { get; set; }
        DbSet<CourtVideoModel> CourtVideos { get; set; }
        DbSet<AccountVideoModel> AccountVideos { get; set; }
        DbSet<VideoModel> Videos { get; set; }
        DbSet<ContactModel> Contacts { get; set; }
        DbSet<CourtReviewModel> CourtReivews { get; set; }
        DbSet<PostModel> Posts { get; set; }
        DbSet<PostFile> PostFiles { get; set; }
        DbSet<PlayerModel> Players { get; set; }
        IDbSet<ApplicationUser> Users { get; set; }
        DbSet<IndexedEntityModel> IndexedEntities { get; set; }

        #region Groups
        DbSet<GroupModel> Groups { get; set; }
        DbSet<GroupMemberModel> GroupMembers { get; set; }
        DbSet<JoinGroupRequest> JoinGroupRequests { get; set; }
        #endregion

        #region User Activites
        DbSet<UserActivityModel> UserActivities { get; set; }
        DbSet<UserPostActivityModel> UserPostActivities { get; set; }
        DbSet<UserAccountActivityModel> UserAccountActivities { get; set; }
        DbSet<AccountPhotoActivityModel> AccountPhotoActivities { get; set; }
        DbSet<AccountVideoActivityModel> AccountVideoActivities { get; set; }
        DbSet<UserContactActivityModel> UserContactActivities { get; set; }
        // COURTS
        DbSet<UserCourtActivityModel> UserCourtActivities { get; set; }
        DbSet<CourtVideoActivityModel> CourtVideoActivities { get; set; }
        DbSet<CourtPhotoActivityModel> CourtPhotoActivities { get; set; }
        //GAMES
        DbSet<UserGameActivityModel> UserGameActivities { get; set; }
        //TEAMS
        DbSet<UserTeamActivityModel> UserTeamActivities { get; set; }
        DbSet<UserJoinTeamRequestActivityModel> UserJoinTeamRequestActivities { get; set; }
        DbSet<TeamPhotoActivityModel> TeamPhotoActivities { get; set; }
        #endregion

        #region Logs
        DbSet<ClientLogModel> ErrorLogs { get; set; }
        DbSet<ExceptionLog> ExceptionLogs { get; set; }
        #endregion
        #region Notifications
        DbSet<NotificationModel> Notifications { get; set; }
        DbSet<NewBookingNotificationModel> NewBookingNotifications { get; set; }
        DbSet<NewGameNotificationModel> NewGameNotifications { get; set; }
        DbSet<UpdateGameNotificationModel> UpdateGameNotifications { get; set; }
        DbSet<JoinTeamRequestNotificationModel> JoinTeamRequestNotifications { get; set; }
        #endregion
        DbEntityEntry Entry(object entity);
        DbEntityEntry SetEntityState(object entity, EntityState state);
        int SaveChanges();
    }

    //This serves as our main connection to our database and Identity tables
    //All Identity tranactions will run on this context
    //You can think about IdentityDbContext class as special version of the traditional “DbContext” Class
    //responsible for handling Identity transactions
    public class AuthContext : ApplicationDbContext, IAuthContext
    {
        public AuthContext()
            : base("AuthContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TournamentTeamModel>()
                .HasKey(e => new { e.TeamId, e.TournamentId });

            modelBuilder.Entity<TournamentPlayerModel>()
                .HasKey(e => new { e.AccountId, e.TournamentId });

            modelBuilder.Entity<TeamStatsModel>()
                .HasKey(e => e.TeamId);

            //modelBuilder.Entity<MultimediaModel>()
            //    .ToTable("Multimedia");

            modelBuilder.Entity<MultimediaModel>()
                .ToTable("Multimedia");

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<PlayerModel> Players { get; set; }
        public DbSet<AccountModel> Accounts { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<LeagueModel> Leagues { get; set; }
        public DbSet<SeasonModel> Seasons { get; set; }

        #region Tournaments
        public DbSet<TournamentModel> Tournaments { get; set; }
        public DbSet<TournamentTeamModel> TournamentTeams { get; set; }
        public DbSet<TournamentStageModel> TournamentStages { get; set; }
        public DbSet<StageTeamModel> StageTeams { get; set; }
        public DbSet<StageBracketModel> StageBrackets { get; set; }
        public DbSet<TournamentPhotoModel> TournamentPhotos { get; set; }
        public DbSet<TournamentPlayerModel> TournamentPlayers { get; set; }
        #endregion

        #region Chats
        public DbSet<ChatModel> Chats { get; set; }
        public DbSet<ChatParticipantModel> ChatParticipants { get; set; }
        public DbSet<ParticipantMessageModel> ParticipantMessages { get; set; }
        public DbSet<MessageModel> Messages { get; set; }
        public DbSet<MessageMediaModel> MessageMedia { get; set; }
        #endregion

        public DbSet<JoinTournamentRequestModel> JoinTournamentRequests { get; set; }
        public DbSet<CourtModel> Courts { get; set; }
        public DbSet<CourtFollowingModel> CourtFollowings { get; set; }
        public DbSet<SettingModel> Settings { get; set; }
        public DbSet<MultimediaModel> Multimedia { get; set; }
        public DbSet<CourtPhotoModel> CourtPhotos { get; set; }
        public DbSet<BookingModel> Bookings { get; set; }
        public DbSet<GameModel> Games { get; set; }
        public DbSet<GameTeamModel> GameTeams { get; set; }
        public DbSet<GamePlayerModel> GamePlayers { get; set; }
        public DbSet<PlayerStatsModel> PlayerStats { get; set; }
        public DbSet<TeamStatsModel> TeamStats { get; set; }
        public DbSet<FoulModel> Fouls { get; set; }

        #region Game Events
        public DbSet<MemberFoulModel> MemberFouls { get; set; }
        public DbSet<GameEventModel> GameEvents { get; set; }
        public DbSet<ShotModel> Shots { get; set; }
        #endregion

        public DbSet<TeamModel> Teams { get; set; }
        public DbSet<TeamMembershipModel> TeamMembers { get; set; }
        public DbSet<TeamPhotoActivityModel> TeamPhotoActivities { get; set; }
        public DbSet<JoinTeamRequestModel> JoinTeamRequests { get; set; }
        public DbSet<UserPermissionModel> UserPermissions { get; set; }
        public DbSet<AccountPhotoModel> AccountPhotos { get; set; }
        public DbSet<AccountHighlightModel> AccountHighlights { get; set; }
        public DbSet<TeamPhotoModel> TeamPhotos { get; set; }
        public DbSet<CourtVideoModel> CourtVideos { get; set; }
        public DbSet<AccountVideoModel> AccountVideos { get; set; }
        public DbSet<VideoModel> Videos { get; set; }
        public DbSet<ContactModel> Contacts { get; set; }
        public DbSet<CourtReviewModel> CourtReivews { get; set; }
        public DbSet<PostModel> Posts { get; set; }
        public DbSet<PostFile> PostFiles { get; set; }
        public DbSet<IndexedEntityModel> IndexedEntities { get; set; }

        public DbEntityEntry SetEntityState(object entity, EntityState state)
        {
            if (entity == null) return null;

            var entry = base.Entry(entity);
            entry.State = state;
            return entry;
        }

        #region Groups
        public DbSet<GroupModel> Groups { get; set; }
        public DbSet<GroupMemberModel> GroupMembers { get; set; }
        public DbSet<JoinGroupRequest> JoinGroupRequests { get; set; }
        #endregion

        #region User Activites
        public DbSet<UserActivityModel> UserActivities { get; set; }
        public DbSet<UserPostActivityModel> UserPostActivities { get; set; }
        public DbSet<UserAccountActivityModel> UserAccountActivities { get; set; }
        public DbSet<AccountPhotoActivityModel> AccountPhotoActivities { get; set; }
        public DbSet<AccountVideoActivityModel> AccountVideoActivities { get; set; }
        public DbSet<UserContactActivityModel> UserContactActivities { get; set; }
        // COURTS
        public DbSet<UserCourtActivityModel> UserCourtActivities { get; set; }
        public DbSet<CourtVideoActivityModel> CourtVideoActivities { get; set; }
        public DbSet<CourtPhotoActivityModel> CourtPhotoActivities { get; set; }
        // GAMES
        public DbSet<UserGameActivityModel> UserGameActivities { get; set; }
        // TEAMS
        public DbSet<UserTeamActivityModel> UserTeamActivities { get; set; }
        public DbSet<UserJoinTeamRequestActivityModel> UserJoinTeamRequestActivities { get; set; }
        #endregion

        #region Logs
        public DbSet<ClientLogModel> ErrorLogs { get; set; }
        public DbSet<ExceptionLog> ExceptionLogs { get; set; }
        #endregion

        #region Notifications
        public DbSet<NotificationModel> Notifications { get; set; }
        public DbSet<NewBookingNotificationModel> NewBookingNotifications { get; set; }
        public DbSet<NewGameNotificationModel> NewGameNotifications { get; set; }
        public DbSet<UpdateGameNotificationModel> UpdateGameNotifications { get; set; }
        public DbSet<JoinTeamRequestNotificationModel> JoinTeamRequestNotifications { get; set; }
        #endregion

        public static AuthContext Create()
        {
            return new AuthContext();
        }
    }
}
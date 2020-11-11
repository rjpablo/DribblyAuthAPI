namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Alter_Columns : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Accounts", "IdentityUserId", c => c.Long(nullable: false));
            AlterColumn("dbo.AspNetUsers", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.AspNetUserClaims", "UserId", c => c.Long(nullable: false));
            AlterColumn("dbo.AspNetUserLogins", "UserId", c => c.Long(nullable: false));
            AlterColumn("dbo.AspNetUserRoles", "UserId", c => c.Long(nullable: false));
            AlterColumn("dbo.AspNetUserRoles", "RoleId", c => c.Long(nullable: false));
            AlterColumn("dbo.NewBookingNotifications", "BookedById", c => c.String());
            AlterColumn("dbo.AspNetRoles", "Id", c => c.Long(nullable: false, identity: true));
        }

        public override void Down()
        {
            AlterColumn("dbo.AspNetRoles", "Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.NewBookingNotifications", "BookedById", c => c.String(maxLength: 128));
            AlterColumn("dbo.AspNetUserRoles", "RoleId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.AspNetUserRoles", "UserId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.AspNetUserLogins", "UserId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.AspNetUserClaims", "UserId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.AspNetUsers", "Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Accounts", "IdentityUserId", c => c.String(maxLength: 128));
        }
    }
}

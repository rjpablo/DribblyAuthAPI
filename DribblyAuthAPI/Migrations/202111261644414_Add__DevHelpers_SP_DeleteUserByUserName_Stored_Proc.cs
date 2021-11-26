namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add__DevHelpers_SP_DeleteUserByUserName_Stored_Proc : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(
            "dbo._DevHelpers_SP_DeleteUserByUserName",
             p => new
             {
                 userName = p.String()
             },
            body:
                @"
                -- This only deletes a user that has just registered and has not added any other data
	            DECLARE @userId BIGINT;

	            SELECT @userId = u.Id
	            FROM AspNetUsers u
	            WHERE u.UserName = @userName;

	            DELETE FROM IndexedEntities
	            WHERE id = @userId;

	            DELETE FROM Accounts
	            WHERE IdentityUserId = @userId;

	            DELETE FROM AspNetUserLogins
	            WHERE UserId = @userId;

	            DELETE FROM AspNetUserClaims
	            WHERE UserId = @userId;

	            DELETE FROM AspNetUserRoles
	            WHERE UserId = @userId;

	            DELETE FROM AspNetUsers
	            WHERE Id = @userId;"
        );
        }
        
        public override void Down()
        {
            DropStoredProcedure("dbo._DevHelpers_SP_DeleteUserByUserName");
        }
    }
}

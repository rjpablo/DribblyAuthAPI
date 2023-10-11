namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_UserPermissionsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserPermissions",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        PermissionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.PermissionId });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserPermissions");
        }
    }
}

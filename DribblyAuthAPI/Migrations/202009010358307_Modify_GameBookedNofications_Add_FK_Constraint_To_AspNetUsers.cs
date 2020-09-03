namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modify_GameBookedNofications_Add_FK_Constraint_To_AspNetUsers : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.GameBookedNotifications", "BookedById", c => c.String(maxLength: 128));
            CreateIndex("dbo.GameBookedNotifications", "BookedById");
            AddForeignKey("dbo.GameBookedNotifications", "BookedById", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GameBookedNotifications", "BookedById", "dbo.AspNetUsers");
            DropIndex("dbo.GameBookedNotifications", new[] { "BookedById" });
            AlterColumn("dbo.GameBookedNotifications", "BookedById", c => c.String());
        }
    }
}

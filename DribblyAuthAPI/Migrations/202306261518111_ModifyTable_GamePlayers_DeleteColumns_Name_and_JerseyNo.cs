namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_GamePlayers_DeleteColumns_Name_and_JerseyNo : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GamePlayers", "ProfilePhoto_Id", "dbo.Photos");
            DropIndex("dbo.GamePlayers", new[] { "ProfilePhoto_Id" });
            DropColumn("dbo.GamePlayers", "Name");
            DropColumn("dbo.GamePlayers", "JerseyNo");
            DropColumn("dbo.GamePlayers", "ProfilePhoto_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GamePlayers", "ProfilePhoto_Id", c => c.Long());
            AddColumn("dbo.GamePlayers", "JerseyNo", c => c.Int(nullable: false));
            AddColumn("dbo.GamePlayers", "Name", c => c.String());
            CreateIndex("dbo.GamePlayers", "ProfilePhoto_Id");
            AddForeignKey("dbo.GamePlayers", "ProfilePhoto_Id", "dbo.Photos", "Id");
        }
    }
}

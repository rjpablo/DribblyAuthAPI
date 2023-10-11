namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_GamePlayers_AddColumn_IsInGame : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GamePlayers", "IsInGame", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GamePlayers", "IsInGame");
        }
    }
}

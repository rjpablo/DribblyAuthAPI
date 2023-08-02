namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_GamePlayers_AddColumn_Won : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GamePlayers", "Won", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GamePlayers", "Won");
        }
    }
}

namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_GamePlayers_AddColumn_Blocks : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GamePlayers", "Blocks", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GamePlayers", "Blocks");
        }
    }
}

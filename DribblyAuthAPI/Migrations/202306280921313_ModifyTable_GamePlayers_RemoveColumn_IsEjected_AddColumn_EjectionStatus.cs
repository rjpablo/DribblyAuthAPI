namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_GamePlayers_RemoveColumn_IsEjected_AddColumn_EjectionStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GamePlayers", "EjectionStatus", c => c.Int(nullable: false));
            DropColumn("dbo.GamePlayers", "IsEjected");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GamePlayers", "IsEjected", c => c.Boolean(nullable: false));
            DropColumn("dbo.GamePlayers", "EjectionStatus");
        }
    }
}

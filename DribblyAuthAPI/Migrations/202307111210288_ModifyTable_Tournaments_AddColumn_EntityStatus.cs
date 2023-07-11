namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_Tournaments_AddColumn_EntityStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tournaments", "EntityStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tournaments", "EntityStatus");
        }
    }
}

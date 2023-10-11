namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modify_Table_IndexexEntities_Rename_Column_Status_To_EntityStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "EntityStatus", c => c.Int(nullable: false));
            RenameColumn("dbo.Accounts", "Status", "EntityStatus");
            RenameColumn("dbo.Courts", "Status", "EntityStatus");
            RenameColumn("dbo.IndexedEntities", "Status", "EntityStatus");
            RenameColumn("dbo.Posts", "Status", "EntityStatus");
            RenameColumn("dbo.Teams", "Status", "EntityStatus");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.Teams", "EntityStatus", "Status");
            RenameColumn("dbo.Posts", "EntityStatus", "Status");
            RenameColumn("dbo.IndexedEntities", "EntityStatus", "Status");
            RenameColumn("dbo.Courts", "EntityStatus", "Status");
            RenameColumn("dbo.Accounts", "EntityStatus", "Status");
            DropColumn("dbo.Games", "EntityStatus");
        }
    }
}

namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_IndexedEntities_AddColumn_AdditionalData : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.IndexedEntities", "AdditionalData", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.IndexedEntities", "AdditionalData");
        }
    }
}

namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_Posts_AddColumns_Type_and_AdditionalData : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Posts", "Type", c => c.Int());
            AddColumn("dbo.Posts", "AdditionalData", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Posts", "AdditionalData");
            DropColumn("dbo.Posts", "Type");
        }
    }
}

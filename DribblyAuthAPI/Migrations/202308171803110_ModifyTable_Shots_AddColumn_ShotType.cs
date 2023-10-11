namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_Shots_AddColumn_ShotType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shots", "ShotType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Shots", "ShotType");
        }
    }
}

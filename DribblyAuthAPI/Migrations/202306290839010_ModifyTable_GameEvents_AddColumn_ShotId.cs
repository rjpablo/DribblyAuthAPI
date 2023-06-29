namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_GameEvents_AddColumn_ShotId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GameEvents", "ShotId", c => c.Long());
            CreateIndex("dbo.GameEvents", "ShotId");
            AddForeignKey("dbo.GameEvents", "ShotId", "dbo.GameEvents", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GameEvents", "ShotId", "dbo.GameEvents");
            DropIndex("dbo.GameEvents", new[] { "ShotId" });
            DropColumn("dbo.GameEvents", "ShotId");
        }
    }
}

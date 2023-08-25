namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_GameEvents_ModifyColumn_TeamId_make_nullable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GameEvents", "TeamId", "dbo.Teams");
            DropIndex("dbo.GameEvents", new[] { "TeamId" });
            AlterColumn("dbo.GameEvents", "TeamId", c => c.Long());
            CreateIndex("dbo.GameEvents", "TeamId");
            AddForeignKey("dbo.GameEvents", "TeamId", "dbo.Teams", "Id");
        }
        
        public override void Down()
        {
            Sql("DELETE FROM shots");
            Sql("DELETE FROM memberfouls");
            Sql("DELETE FROM GameEvents");
            DropForeignKey("dbo.GameEvents", "TeamId", "dbo.Teams");
            DropIndex("dbo.GameEvents", new[] { "TeamId" });
            AlterColumn("dbo.GameEvents", "TeamId", c => c.Long(nullable: false));
            CreateIndex("dbo.GameEvents", "TeamId");
            AddForeignKey("dbo.GameEvents", "TeamId", "dbo.Teams", "Id", cascadeDelete: true);
        }
    }
}

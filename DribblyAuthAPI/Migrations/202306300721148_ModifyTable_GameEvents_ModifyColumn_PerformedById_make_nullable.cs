namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_GameEvents_ModifyColumn_PerformedById_make_nullable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GameEvents", "PerformedById", "dbo.Accounts");
            DropIndex("dbo.GameEvents", new[] { "PerformedById" });
            AlterColumn("dbo.GameEvents", "PerformedById", c => c.Long());
            CreateIndex("dbo.GameEvents", "PerformedById");
            AddForeignKey("dbo.GameEvents", "PerformedById", "dbo.Accounts", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GameEvents", "PerformedById", "dbo.Accounts");
            DropIndex("dbo.GameEvents", new[] { "PerformedById" });
            AlterColumn("dbo.GameEvents", "PerformedById", c => c.Long(nullable: false));
            CreateIndex("dbo.GameEvents", "PerformedById");
            AddForeignKey("dbo.GameEvents", "PerformedById", "dbo.Accounts", "Id", cascadeDelete: true);
        }
    }
}

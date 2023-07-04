namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_Tournaments_AddColumns_DefaultCourtId_and_LogoId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tournaments", "DefaultCourtId", c => c.Long());
            AddColumn("dbo.Tournaments", "LogoId", c => c.Long());
            CreateIndex("dbo.Tournaments", "DefaultCourtId");
            CreateIndex("dbo.Tournaments", "LogoId");
            AddForeignKey("dbo.Tournaments", "DefaultCourtId", "dbo.Courts", "Id");
            AddForeignKey("dbo.Tournaments", "LogoId", "dbo.Photos", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tournaments", "LogoId", "dbo.Photos");
            DropForeignKey("dbo.Tournaments", "DefaultCourtId", "dbo.Courts");
            DropIndex("dbo.Tournaments", new[] { "LogoId" });
            DropIndex("dbo.Tournaments", new[] { "DefaultCourtId" });
            DropColumn("dbo.Tournaments", "LogoId");
            DropColumn("dbo.Tournaments", "DefaultCourtId");
        }
    }
}

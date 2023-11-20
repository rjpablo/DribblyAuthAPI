namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddForeignKey_PlayerCity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "CityId", c => c.Long());
            CreateIndex("dbo.Players", "CityId");
            AddForeignKey("dbo.Players", "CityId", "dbo.Cities", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Players", "CityId", "dbo.Cities");
            DropIndex("dbo.Players", new[] { "CityId" });
            DropColumn("dbo.Players", "CityId");
        }
    }
}

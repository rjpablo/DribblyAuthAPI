namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_Table_Games_Add_Column_Status : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courts", "status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Courts", "status");
        }
    }
}

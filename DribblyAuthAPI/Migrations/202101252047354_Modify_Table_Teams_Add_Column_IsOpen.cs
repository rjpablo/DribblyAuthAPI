namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modify_Table_Teams_Add_Column_IsOpen : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Teams", "IsOpen", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Teams", "IsOpen");
        }
    }
}

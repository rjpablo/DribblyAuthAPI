namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_Notifications_AddColumn_AdditionalInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notifications", "AdditionalInfo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Notifications", "AdditionalInfo");
        }
    }
}

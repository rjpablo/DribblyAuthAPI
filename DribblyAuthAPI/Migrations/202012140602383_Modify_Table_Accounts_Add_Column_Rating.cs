namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modify_Table_Accounts_Add_Column_Rating : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "Rating", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accounts", "Rating");
        }
    }
}

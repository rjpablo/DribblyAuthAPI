namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Move_Username_Column_From_PlayerModel_To_AccountModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "Username", c => c.String(maxLength: 30));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accounts", "Username");
        }
    }
}

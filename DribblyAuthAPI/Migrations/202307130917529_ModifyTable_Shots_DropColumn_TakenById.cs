namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_Shots_DropColumn_TakenById : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Shots", "TakenById");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Shots", "TakenById", c => c.Long());
        }
    }
}

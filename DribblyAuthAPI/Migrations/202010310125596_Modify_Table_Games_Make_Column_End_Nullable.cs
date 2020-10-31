namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modify_Table_Games_Make_Column_End_Nullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Games", "End", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Games", "End", c => c.DateTime(nullable: false));
        }
    }
}

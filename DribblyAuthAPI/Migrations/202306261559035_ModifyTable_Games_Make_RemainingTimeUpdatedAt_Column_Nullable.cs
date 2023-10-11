namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_Games_Make_RemainingTimeUpdatedAt_Column_Nullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Games", "RemainingTimeUpdatedAt", c => c.DateTime());
        }
        
        public override void Down()
        {
            Sql("DELETE FROM UserGameActivities");
            Sql("DELETE FROM Games");
            AlterColumn("dbo.Games", "RemainingTimeUpdatedAt", c => c.DateTime(nullable: false));
        }
    }
}

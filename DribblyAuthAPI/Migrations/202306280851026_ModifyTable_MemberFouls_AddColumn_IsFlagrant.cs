namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_MemberFouls_AddColumn_IsFlagrant : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MemberFouls", "IsFlagrant", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MemberFouls", "IsFlagrant");
        }
    }
}

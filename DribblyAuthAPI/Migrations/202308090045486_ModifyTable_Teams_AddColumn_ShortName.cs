namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_Teams_AddColumn_ShortName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Teams", "ShortName", c => c.String(maxLength: 15));
            Sql("UPDATE teams SET shortName = SUBSTRING(Name, 1, 15);");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Teams", "ShortName");
        }
    }
}

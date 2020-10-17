namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modify_Courts_Rating_Column_DataType_To_Double : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Courts", "Rating", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Courts", "Rating", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}

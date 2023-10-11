namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameTable_Photos_to_Multimedia : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Photos", newName: "Multimedia");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Multimedia", newName: "Photos");
        }
    }
}

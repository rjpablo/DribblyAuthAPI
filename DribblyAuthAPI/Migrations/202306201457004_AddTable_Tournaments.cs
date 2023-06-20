namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTable_Tournaments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tournaments",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        AddedById = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Tournaments");
        }
    }
}

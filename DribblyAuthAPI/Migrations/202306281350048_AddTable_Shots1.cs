namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTable_Shots1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Shots",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        Points = c.Int(nullable: false),
                        IsMiss = c.Boolean(nullable: false),
                        TakenById = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GameEvents", t => t.Id)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Shots", "Id", "dbo.GameEvents");
            DropIndex("dbo.Shots", new[] { "Id" });
            DropTable("dbo.Shots");
        }
    }
}

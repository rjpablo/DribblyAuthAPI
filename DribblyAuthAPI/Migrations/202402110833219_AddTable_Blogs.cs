namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTable_Blogs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Blogs",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Slug = c.String(),
                        IconUrl = c.String(),
                        EntityStatus = c.Int(nullable: false),
                        Description = c.String(),
                        AddedById = c.Long(nullable: false),
                        Content = c.String(),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AddedById, cascadeDelete: true)
                .Index(t => t.AddedById);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Blogs", "AddedById", "dbo.Accounts");
            DropIndex("dbo.Blogs", new[] { "AddedById" });
            DropTable("dbo.Blogs");
        }
    }
}

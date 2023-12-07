namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTable_Comments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Message = c.String(),
                        AddedById = c.Long(nullable: false),
                        CommentedOnType = c.Int(nullable: false),
                        CommentedOnId = c.Long(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Players", t => t.AddedById)
                .Index(t => t.AddedById);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "AddedById", "dbo.Players");
            DropIndex("dbo.Comments", new[] { "AddedById" });
            DropTable("dbo.Comments");
        }
    }
}

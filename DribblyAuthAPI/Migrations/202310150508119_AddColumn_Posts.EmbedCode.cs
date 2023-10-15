namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumn_PostsEmbedCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Posts", "EmbedCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Posts", "EmbedCode");
        }
    }
}

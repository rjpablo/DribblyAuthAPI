namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTable_Posts_ModifyColumn_AddedById_add_FK_reference_to_Accounts_table : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Posts", "AddedById");
            AddForeignKey("dbo.Posts", "AddedById", "dbo.Accounts", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Posts", "AddedById", "dbo.Accounts");
            DropIndex("dbo.Posts", new[] { "AddedById" });
        }
    }
}

namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatTable_AccountFlags : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountFlags",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 128),
                        AccountId = c.Long(nullable: false),
                        Value = c.String(),
                    })
                .PrimaryKey(t => new { t.Key, t.AccountId })
                .ForeignKey("dbo.Accounts", t => t.AccountId, cascadeDelete: true)
                .Index(t => t.AccountId);
            Sql(@"MERGE AccountFlags AS Target
                  USING Accounts AS Source
                  ON Target.AccountId = Source.Id AND Target.[Key] = 'upload_primary_photo' AND Source.ProfilePhotoId IS NOT NULL
                  WHEN NOT MATCHED BY Target THEN
                    INSERT(AccountId, [Key], Value)
                    VALUES(Source.Id, 'upload_primary_photo', null);");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccountFlags", "AccountId", "dbo.Accounts");
            DropIndex("dbo.AccountFlags", new[] { "AccountId" });
            DropTable("dbo.AccountFlags");
        }
    }
}

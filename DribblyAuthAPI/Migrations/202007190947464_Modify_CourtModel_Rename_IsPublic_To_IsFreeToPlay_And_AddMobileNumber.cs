namespace DribblyAuthAPI.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Modify_CourtModel_Rename_IsPublic_To_IsFreeToPlay_And_AddMobileNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courts", "MobileNumber", c => c.String());
            RenameColumn("dbo.Courts", "IsPublic", "IsFreeToPlay");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.Courts", "IsFreeToPlay", "IsPublic");
            DropColumn("dbo.Courts", "MobileNumber");
        }
    }
}

namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_ClientLogsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClientLogs",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Message = c.String(),
                        ErrorMessage = c.String(),
                        Stack = c.String(),
                        Url = c.String(),
                        ErrorCode = c.Int(nullable: false),
                        LineNo = c.Int(nullable: false),
                        Column = c.Int(nullable: false),
                        LoggedBy = c.String(),
                        Browser = c.String(),
                        Os = c.String(),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ClientLogs");
        }
    }
}

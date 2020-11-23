namespace DribblyAuthAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Add_Table_IndexEntities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IndexedEntities",
                c => new
                {
                    Id = c.Long(nullable: false),
                    Type = c.Int(nullable: false),
                    Name = c.String(),
                    IconUrl = c.String(),
                    Status = c.Int(nullable: false),
                    Description = c.String(),
                    DateAdded = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => new { t.Id, t.Type });
            Sql(@"INSERT INTO indexedentities 
                                (id, 
                                 type, 
                                 NAME, 
                                 iconurl, 
                                 status, 
                                 description, 
                                 dateadded) 
                    -- Accounts  
                    SELECT c.id, 
                           0     [type], 
                           c.username, 
                           p.url AS iconUrl, 
                           a.status, 
                           ''    AS description, 
                           a.dateadded 
                    FROM   accounts a 
                           INNER JOIN aspnetusers AS c 
                                   ON a.identityuserid = c.id 
                           LEFT JOIN photos AS p 
                                  ON a.profilephotoid = p.id 
                    -- Courts  
                    UNION 
                    SELECT c.id, 
                           1                [type], 
                           c.NAME, 
                           p.url            AS iconUrl, 
                           c.status, 
                           c.additionalinfo AS description, 
                           c.dateadded 
                    FROM   courts AS c 
                           LEFT JOIN photos AS p 
                                  ON c.primaryphotoid = p.id 
                    -- Games 
                    UNION 
                    SELECT g.id, 
                           2       [type], 
                           g.title AS NAME, 
                           ''      AS iconUrl, 
                           g.status, 
                           ''      AS description, 
                           g.dateadded 
                    FROM   games AS g 
                    -- POSTS 
                    UNION 
                    SELECT p.id, 
                           4         [type], 
                           ''        AS NAME, 
                           ''        AS iconUrl, 
                           p.status, 
                           p.content AS description, 
                           p.dateadded 
                    FROM   posts AS p; ");
        }

        public override void Down()
        {
            DropTable("dbo.IndexedEntities");
        }
    }
}

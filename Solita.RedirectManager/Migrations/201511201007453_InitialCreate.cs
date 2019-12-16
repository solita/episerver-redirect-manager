namespace Solita.RedirectManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SOLITA_Rewritemapping",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RequestUrl = c.String(maxLength: 450),
                        RedirectUrl = c.String(),
                        SortOrder = c.Int(nullable: false),
                        IsWildcard = c.Boolean(nullable: false),
                        IsAbsolute = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.RequestUrl, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.SOLITA_Rewritemapping", new[] { "RequestUrl" });
            DropTable("dbo.SOLITA_Rewritemapping");
        }
    }
}

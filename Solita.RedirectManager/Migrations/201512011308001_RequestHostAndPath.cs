namespace Solita.RedirectManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequestHostAndPath : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.SOLITA_Rewritemapping", new[] { "RequestUrl" });
            AddColumn("dbo.SOLITA_Rewritemapping", "RequestHost", c => c.String());
            AddColumn("dbo.SOLITA_Rewritemapping", "RequestPath", c => c.String());
            DropColumn("dbo.SOLITA_Rewritemapping", "RequestUrl");
            DropColumn("dbo.SOLITA_Rewritemapping", "IsAbsolute");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SOLITA_Rewritemapping", "IsAbsolute", c => c.Boolean(nullable: false));
            AddColumn("dbo.SOLITA_Rewritemapping", "RequestUrl", c => c.String(maxLength: 450));
            DropColumn("dbo.SOLITA_Rewritemapping", "RequestPath");
            DropColumn("dbo.SOLITA_Rewritemapping", "RequestHost");
            CreateIndex("dbo.SOLITA_Rewritemapping", "RequestUrl", unique: true);
        }
    }
}

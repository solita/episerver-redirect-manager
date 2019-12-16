namespace Solita.RedirectManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SOLITA_Rewritemapping", "UpdatedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.SOLITA_Rewritemapping", "CreatedAt", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SOLITA_Rewritemapping", "CreatedAt");
            DropColumn("dbo.SOLITA_Rewritemapping", "UpdatedAt");
        }
    }
}

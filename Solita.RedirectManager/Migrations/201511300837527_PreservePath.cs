namespace Solita.RedirectManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PreservePath : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SOLITA_Rewritemapping", "PreservePath", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SOLITA_Rewritemapping", "PreservePath");
        }
    }
}

namespace Solita.RedirectManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LastUsed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SOLITA_Rewritemapping", "LastUsed", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SOLITA_Rewritemapping", "LastUsed");
        }
    }
}

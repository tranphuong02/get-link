namespace DatabaseAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDatabase : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Requests", "RequestToken");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Requests", "RequestToken", c => c.String());
        }
    }
}

namespace DatabaseAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        Password = c.String(),
                        UserType = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Requests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Ip = c.String(),
                        RequestUrl = c.String(),
                        RequestPassword = c.String(),
                        RequestType = c.Int(nullable: false),
                        RequestToken = c.String(),
                        ResultUrl = c.String(),
                        ResultName = c.String(),
                        ResultSize = c.String(),
                        ResultAdsUrl = c.String(),
                        ResultAdsType = c.String(),
                        CustomerId = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .Index(t => t.CustomerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Requests", "CustomerId", "dbo.Customers");
            DropIndex("dbo.Requests", new[] { "CustomerId" });
            DropTable("dbo.Requests");
            DropTable("dbo.Customers");
        }
    }
}

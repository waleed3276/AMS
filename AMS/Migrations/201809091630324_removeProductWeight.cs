namespace AMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeProductWeight : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Products", "Product_Weight");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "Product_Weight", c => c.Int(nullable: false));
        }
    }
}

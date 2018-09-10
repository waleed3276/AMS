namespace AMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class productSizeFloat : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ProductSizes", "ProductSize_Height", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.ProductSizes", "ProductSize_Width", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.ProductSizes", "ProductSize_Length", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProductSizes", "ProductSize_Length", c => c.Int(nullable: false));
            AlterColumn("dbo.ProductSizes", "ProductSize_Width", c => c.Int(nullable: false));
            AlterColumn("dbo.ProductSizes", "ProductSize_Height", c => c.Int(nullable: false));
        }
    }
}

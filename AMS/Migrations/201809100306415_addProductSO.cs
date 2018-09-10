namespace AMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addProductSO : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SaleOrder_Ch", "Product_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.SaleOrder_Ch", "Product_Id");
            AddForeignKey("dbo.SaleOrder_Ch", "Product_Id", "dbo.Products", "Product_Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SaleOrder_Ch", "Product_Id", "dbo.Products");
            DropIndex("dbo.SaleOrder_Ch", new[] { "Product_Id" });
            DropColumn("dbo.SaleOrder_Ch", "Product_Id");
        }
    }
}

namespace AMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSalesTax : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SalesTax_Ch",
                c => new
                    {
                        STC_Id = c.Int(nullable: false, identity: true),
                        SOC_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.STC_Id)
                .ForeignKey("dbo.SaleOrder_Ch", t => t.SOC_Id, cascadeDelete: true)
                .Index(t => t.SOC_Id);
            
            CreateTable(
                "dbo.SalesTax_Pt",
                c => new
                    {
                        STP_Id = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        STP_DeliveryChallanNo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        STP_TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        STP_GST = c.Int(nullable: false),
                        STP_TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        STP_TotalReceived = c.Decimal(nullable: false, precision: 18, scale: 2),
                        STP_Date = c.DateTime(nullable: false),
                        STP_Status = c.String(),
                        Customer_Customer_Id = c.Int(),
                    })
                .PrimaryKey(t => t.STP_Id)
                .ForeignKey("dbo.Customers", t => t.Customer_Customer_Id)
                .Index(t => t.Customer_Customer_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SalesTax_Pt", "Customer_Customer_Id", "dbo.Customers");
            DropForeignKey("dbo.SalesTax_Ch", "SOC_Id", "dbo.SaleOrder_Ch");
            DropIndex("dbo.SalesTax_Pt", new[] { "Customer_Customer_Id" });
            DropIndex("dbo.SalesTax_Ch", new[] { "SOC_Id" });
            DropTable("dbo.SalesTax_Pt");
            DropTable("dbo.SalesTax_Ch");
        }
    }
}

namespace AMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GatePass : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GatePass_Pt", "Vendor_Id", "dbo.Vendors");
            DropForeignKey("dbo.GatePass_Ch", "GPP_Id", "dbo.GatePass_Pt");
            DropForeignKey("dbo.GatePass_Ch", "Product_Id", "dbo.Products");
            DropIndex("dbo.GatePass_Ch", new[] { "GPP_Id" });
            DropIndex("dbo.GatePass_Ch", new[] { "Product_Id" });
            DropIndex("dbo.GatePass_Pt", new[] { "Vendor_Id" });
            CreateTable(
                "dbo.GatePasses",
                c => new
                    {
                        GatePass_Id = c.Int(nullable: false, identity: true),
                        Customer_Id = c.Int(nullable: false),
                        PurchaseOrder_PtId = c.Int(nullable: false),
                        GatePass_No = c.String(),
                        GatePass_Date = c.DateTime(nullable: false),
                        GatePass_Status = c.Boolean(nullable: false),
                        PurchaseOrder_Pt_POP_Id = c.Int(),
                    })
                .PrimaryKey(t => t.GatePass_Id)
                .ForeignKey("dbo.Customers", t => t.Customer_Id, cascadeDelete: true)
                .ForeignKey("dbo.PurchaseOrder_Pt", t => t.PurchaseOrder_Pt_POP_Id)
                .Index(t => t.Customer_Id)
                .Index(t => t.PurchaseOrder_Pt_POP_Id);
            
            DropTable("dbo.GatePass_Ch");
            DropTable("dbo.GatePass_Pt");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.GatePass_Pt",
                c => new
                    {
                        GPP_Id = c.Int(nullable: false, identity: true),
                        Vendor_Id = c.Int(nullable: false),
                        GPP_TotalQuantity = c.Int(nullable: false),
                        GPP_TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GPP_TotalPaid = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GPP_Date = c.DateTime(nullable: false),
                        GPP_ModificationDate = c.DateTime(nullable: false),
                        GPP_Charges = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GPP_TaxPercent = c.Int(nullable: false),
                        GPP_TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GPP_No = c.String(),
                        GPP_Status = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.GPP_Id);
            
            CreateTable(
                "dbo.GatePass_Ch",
                c => new
                    {
                        GPC_Id = c.Int(nullable: false, identity: true),
                        GPP_Id = c.Int(nullable: false),
                        Product_Id = c.Int(nullable: false),
                        GPC_Quantity = c.Int(nullable: false),
                        GPC_Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GPC_Description = c.String(),
                        GPC_GST = c.Int(nullable: false),
                        GPC_Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GPC_Unit = c.String(),
                    })
                .PrimaryKey(t => t.GPC_Id);
            
            DropForeignKey("dbo.GatePasses", "PurchaseOrder_Pt_POP_Id", "dbo.PurchaseOrder_Pt");
            DropForeignKey("dbo.GatePasses", "Customer_Id", "dbo.Customers");
            DropIndex("dbo.GatePasses", new[] { "PurchaseOrder_Pt_POP_Id" });
            DropIndex("dbo.GatePasses", new[] { "Customer_Id" });
            DropTable("dbo.GatePasses");
            CreateIndex("dbo.GatePass_Pt", "Vendor_Id");
            CreateIndex("dbo.GatePass_Ch", "Product_Id");
            CreateIndex("dbo.GatePass_Ch", "GPP_Id");
            AddForeignKey("dbo.GatePass_Ch", "Product_Id", "dbo.Products", "Product_Id", cascadeDelete: true);
            AddForeignKey("dbo.GatePass_Ch", "GPP_Id", "dbo.GatePass_Pt", "GPP_Id", cascadeDelete: true);
            AddForeignKey("dbo.GatePass_Pt", "Vendor_Id", "dbo.Vendors", "Vendor_Id", cascadeDelete: true);
        }
    }
}

namespace AMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class docNoAndDelDate0 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PurchaseOrder_Pt", "POP_DeliveryDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Invoices", "Invoice_DocumentNo", c => c.Int(nullable: false));
            AddColumn("dbo.SaleOrder_Pt", "SOP_DeliveryDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SaleOrder_Pt", "POP_DeliveryDate");
            DropColumn("dbo.Invoices", "Invoice_DocumentNo");
            DropColumn("dbo.PurchaseOrder_Pt", "POP_DeliveryDate");
        }
    }
}

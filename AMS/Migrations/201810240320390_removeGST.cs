namespace AMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeGST : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.PurchaseOrder_Pt", "POP_GST");
            DropColumn("dbo.SaleOrder_Pt", "SOP_GST");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SaleOrder_Pt", "SOP_GST", c => c.Int(nullable: false));
            AddColumn("dbo.PurchaseOrder_Pt", "POP_GST", c => c.Int(nullable: false));
        }
    }
}

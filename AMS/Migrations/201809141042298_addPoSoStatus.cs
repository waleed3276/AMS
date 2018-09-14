namespace AMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPoSoStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PurchaseOrder_Pt", "POP_Status", c => c.String());
            AddColumn("dbo.SaleOrder_Pt", "SOP_Status", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SaleOrder_Pt", "SOP_Status");
            DropColumn("dbo.PurchaseOrder_Pt", "POP_Status");
        }
    }
}

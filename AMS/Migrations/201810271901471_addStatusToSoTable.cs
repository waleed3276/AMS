namespace AMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addStatusToSoTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SaleOrder_Ch", "SOC_SalesTaxStatus", c => c.String());
            AlterColumn("dbo.SalesTax_Pt", "STP_Status", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SalesTax_Pt", "STP_Status", c => c.String());
            DropColumn("dbo.SaleOrder_Ch", "SOC_SalesTaxStatus");
        }
    }
}

namespace AMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dcToString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SalesTax_Pt", "STP_DeliveryChallanNo", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SalesTax_Pt", "STP_DeliveryChallanNo", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}

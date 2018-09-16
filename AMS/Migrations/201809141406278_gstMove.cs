namespace AMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gstMove : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SaleOrder_Pt", "SOP_GST", c => c.Int(nullable: false));
            DropColumn("dbo.SaleOrder_Ch", "SOC_GST");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SaleOrder_Ch", "SOC_GST", c => c.Int(nullable: false));
            DropColumn("dbo.SaleOrder_Pt", "SOP_GST");
        }
    }
}

namespace AMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class docNoAndDelDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SaleOrder_Pt", "SOP_DeliveryDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.SaleOrder_Pt", "POP_DeliveryDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SaleOrder_Pt", "POP_DeliveryDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.SaleOrder_Pt", "SOP_DeliveryDate");
        }
    }
}

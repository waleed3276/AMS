namespace AMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salesTaxRelation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalesTax_Ch", "STP_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.SalesTax_Ch", "STP_Id");
            AddForeignKey("dbo.SalesTax_Ch", "STP_Id", "dbo.SalesTax_Pt", "STP_Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SalesTax_Ch", "STP_Id", "dbo.SalesTax_Pt");
            DropIndex("dbo.SalesTax_Ch", new[] { "STP_Id" });
            DropColumn("dbo.SalesTax_Ch", "STP_Id");
        }
    }
}

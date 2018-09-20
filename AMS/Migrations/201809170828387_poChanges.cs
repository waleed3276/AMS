namespace AMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class poChanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PurchaseOrder_Ch", "PurchaseOrder_Pt_PO_Id", "dbo.PurchaseOrder_Pt");
            DropIndex("dbo.PurchaseOrder_Ch", new[] { "PurchaseOrder_Pt_PO_Id" });
            DropColumn("dbo.PurchaseOrder_Ch", "POP_Id");
            RenameColumn(table: "dbo.PurchaseOrder_Ch", name: "PurchaseOrder_Pt_PO_Id", newName: "POP_Id");
            DropPrimaryKey("dbo.PurchaseOrder_Pt");
                DropColumn("dbo.PurchaseOrder_Pt", "PO_Id");
            AddColumn("dbo.PurchaseOrder_Pt", "POP_Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.PurchaseOrder_Pt", "POP_GST", c => c.Int(nullable: false));
            AlterColumn("dbo.PurchaseOrder_Ch", "POP_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.SaleOrder_Ch", "SOC_ItemCode", c => c.String());
            AddPrimaryKey("dbo.PurchaseOrder_Pt", "POP_Id");
            CreateIndex("dbo.PurchaseOrder_Ch", "POP_Id");
            AddForeignKey("dbo.PurchaseOrder_Ch", "POP_Id", "dbo.PurchaseOrder_Pt", "POP_Id", cascadeDelete: true);
            DropColumn("dbo.PurchaseOrder_Ch", "POC_GST");
            //DropColumn("dbo.PurchaseOrder_Pt", "PO_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PurchaseOrder_Pt", "PO_Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.PurchaseOrder_Ch", "POC_GST", c => c.Int(nullable: false));
            DropForeignKey("dbo.PurchaseOrder_Ch", "POP_Id", "dbo.PurchaseOrder_Pt");
            DropIndex("dbo.PurchaseOrder_Ch", new[] { "POP_Id" });
            DropPrimaryKey("dbo.PurchaseOrder_Pt");
            AlterColumn("dbo.SaleOrder_Ch", "SOC_ItemCode", c => c.Int(nullable: false));
            AlterColumn("dbo.PurchaseOrder_Ch", "POP_Id", c => c.Int());
            DropColumn("dbo.PurchaseOrder_Pt", "POP_GST");
            DropColumn("dbo.PurchaseOrder_Pt", "POP_Id");
            AddPrimaryKey("dbo.PurchaseOrder_Pt", "PO_Id");
            RenameColumn(table: "dbo.PurchaseOrder_Ch", name: "POP_Id", newName: "PurchaseOrder_Pt_PO_Id");
            AddColumn("dbo.PurchaseOrder_Ch", "POP_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.PurchaseOrder_Ch", "PurchaseOrder_Pt_PO_Id");
            AddForeignKey("dbo.PurchaseOrder_Ch", "PurchaseOrder_Pt_PO_Id", "dbo.PurchaseOrder_Pt", "PO_Id");
        }
    }
}

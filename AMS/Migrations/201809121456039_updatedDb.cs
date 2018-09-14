namespace AMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedDb : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PurchaseOrder_Ch", "PurchaseOrder_Pt_PO_Id", "dbo.PurchaseOrder_Pt");
            DropIndex("dbo.PurchaseOrder_Ch", new[] { "PurchaseOrder_Pt_PO_Id" });
            DropColumn("dbo.PurchaseOrder_Ch", "POP_Id");
            RenameColumn(table: "dbo.PurchaseOrder_Ch", name: "PurchaseOrder_Pt_PO_Id", newName: "POP_Id");
            DropPrimaryKey("dbo.PurchaseOrder_Pt");
            AddColumn("dbo.PurchaseOrder_Pt", "POP_Id", c => c.Int(nullable: false, identity: false));
            AlterColumn("dbo.PurchaseOrder_Ch", "POP_Id", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.PurchaseOrder_Pt", "POP_Id");
            CreateIndex("dbo.PurchaseOrder_Ch", "POP_Id");
            AddForeignKey("dbo.PurchaseOrder_Ch", "POP_Id", "dbo.PurchaseOrder_Pt", "POP_Id", cascadeDelete: true);
            DropColumn("dbo.PurchaseOrder_Pt", "PO_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PurchaseOrder_Pt", "PO_Id", c => c.Int(nullable: false, identity: false));
            DropForeignKey("dbo.PurchaseOrder_Ch", "POP_Id", "dbo.PurchaseOrder_Pt");
            DropIndex("dbo.PurchaseOrder_Ch", new[] { "POP_Id" });
            DropPrimaryKey("dbo.PurchaseOrder_Pt");
            AlterColumn("dbo.PurchaseOrder_Ch", "POP_Id", c => c.Int());
            DropColumn("dbo.PurchaseOrder_Pt", "POP_Id");
            AddPrimaryKey("dbo.PurchaseOrder_Pt", "PO_Id");
            RenameColumn(table: "dbo.PurchaseOrder_Ch", name: "POP_Id", newName: "PurchaseOrder_Pt_PO_Id");
            AddColumn("dbo.PurchaseOrder_Ch", "POP_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.PurchaseOrder_Ch", "PurchaseOrder_Pt_PO_Id");
            AddForeignKey("dbo.PurchaseOrder_Ch", "PurchaseOrder_Pt_PO_Id", "dbo.PurchaseOrder_Pt", "PO_Id");
        }
    }
}

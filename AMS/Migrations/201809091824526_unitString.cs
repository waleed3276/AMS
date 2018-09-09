namespace AMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class unitString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ProductSizes", "ProductSize_Unit", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProductSizes", "ProductSize_Unit", c => c.Int(nullable: false));
        }
    }
}

namespace AMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class emailPasswordForUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "Customer_Email", c => c.String());
            AddColumn("dbo.Customers", "Customer_Password", c => c.String());
            AddColumn("dbo.Vendors", "Vendor_Email", c => c.String());
            AddColumn("dbo.Vendors", "Vendor_Password", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Vendors", "Vendor_Password");
            DropColumn("dbo.Vendors", "Vendor_Email");
            DropColumn("dbo.Customers", "Customer_Password");
            DropColumn("dbo.Customers", "Customer_Email");
        }
    }
}

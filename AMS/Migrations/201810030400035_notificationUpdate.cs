namespace AMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class notificationUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notifications", "Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Notifications", "Notification_IsSeen", c => c.Boolean(nullable: false));
            CreateIndex("dbo.Notifications", "Id");
            AddForeignKey("dbo.Notifications", "Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notifications", "Id", "dbo.AspNetUsers");
            DropIndex("dbo.Notifications", new[] { "Id" });
            DropColumn("dbo.Notifications", "Notification_IsSeen");
            DropColumn("dbo.Notifications", "Id");
        }
    }
}

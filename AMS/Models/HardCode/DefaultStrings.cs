using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Models.HardCode
{
    public class DefaultStrings
    {
        public string SaleInvoiceType = "SaleOrder";
        public string PurchaseInvoiceType = "PurchaseOrder";
        public string SalesTax_InvoiceType = "SalesTaxInvoice";

        public string Balance_Customer = "CustomerBalance";
        public string Balance_Vendor = "VendorBalance";

        public string Role_Admin = "Admin";
        public string Role_Customer = "Customer";
        public string Role_Vendor = "Vendor";

        public string Status_Complete = "Complete";
        public string Status_Approve = "Approve";
        public string Status_Disapprove = "Disapprove";
        public string Status_Pending = "Pending";
        public string Status_InProcess= "InProcess";
        public string Status_Cancel = "Cancel";

        public string Transaction_DriverExpense = "DriverExpense";
        public string Transaction_Manual = "Manual";
    }
}
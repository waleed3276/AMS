using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AMS.Models
{
    public class SalesTax_Pt
    {
        [Key]
        public int STP_Id { get; set; }

        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public string STP_DeliveryChallanNo { get; set; }

        public decimal STP_TotalAmount { get; set; }

        public int STP_GST { get; set; }

        public decimal STP_TaxAmount { get; set; }

        public decimal STP_TotalReceived { get; set; }

        public DateTime STP_Date { get; set; }

        public bool STP_Status { get; set; }
    }
}
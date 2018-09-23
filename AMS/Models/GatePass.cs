using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AMS.Models
{
    public class GatePass
    {
        [Key]
        public int GatePass_Id { get; set; }

        public int Customer_Id { get; set; }
        public virtual Customer Customer { get; set; }

        public int PurchaseOrder_PtId { get; set; }
        public virtual PurchaseOrder_Pt PurchaseOrder_Pt { get; set; }

        public string GatePass_No { get; set; }

        public DateTime GatePass_Date { get; set; }

        public bool GatePass_Status { get; set; }
    }
}
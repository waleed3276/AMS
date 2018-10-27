using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AMS.Models
{
    public class SalesTax_Ch
    {
        [Key]
        public int STC_Id { get; set; }

        public int STP_Id { get; set; }
        public virtual SalesTax_Pt SalesTax_Pt { get; set; }

        public int SOC_Id { get; set; }
        public virtual SaleOrder_Ch SaleOrder_Ch { get; set; }
    }
}
﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AMS.Models
{
    public class SaleOrder_Ch
    {
        [Key]
        public int SOC_Id { get; set; }

        public int SOP_Id { get; set; }
        public virtual SaleOrder_Pt SaleOrder_Pt { get; set; }

        public int Product_Id { get; set; }
        public virtual Product Product { get; set; }

        public int SOC_Quantity { get; set; }

        public decimal SOC_Rate { get; set; }

        public string SOC_Description { get; set; }

        public decimal SOC_Amount { get; set; }

        public string SOC_ItemCode { get; set; }

        public string SOC_Unit { get; set; }

        public string SOC_SalesTaxStatus { get; set; }
    }
}
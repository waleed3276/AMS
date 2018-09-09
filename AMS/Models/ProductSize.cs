using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AMS.Models
{
    public class ProductSize
    {
        [Key]
        public int ProductSize_Id { get; set; }

        public string ProductSize_Value { get; set; }

        public decimal ProductSize_Height { get; set; }

        public decimal ProductSize_Width { get; set; }

        public decimal ProductSize_Length { get; set; }

        public string ProductSize_Unit { get; set; }

        //public int ProductSize_ItemId { get; set; }

        

    }
}
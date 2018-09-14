using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Models.HardCode
{
    public class OrderNumber
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public long GenerateSaleOrderNumber()
        {
            long soNumber = 100000;
            try
            {
                soNumber = Convert.ToInt64(db.SaleOrder_Pts.Max(m => m.SOP_SO));
            }
            catch (Exception)
            {

            }
            return (soNumber == 100000) ? 100001 : ++soNumber;
        }
    }
}
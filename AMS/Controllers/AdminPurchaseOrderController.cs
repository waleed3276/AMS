using AMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS.Controllers
{
    
    public class AdminPurchaseOrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: AdminPurchaseOrder
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetProducts() { 
            //int id = Session[];
            //int id = 223;
            //var p = db.SaleOrder_Pts.Where(s => s.SOP_Id == id).ToList();
            //var productList= db.SaleOrder_Ches.Where(s=> s.SOP_Id==p.id)
            //var pp = db.SaleOrder_Ches.Where(s=> s.SaleOrder_Pt.SOP_Id==id)

            var product_list = db.Products.ToList();
            return Json(product_list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductDetail(int id)
        {
            var product = db.Products.Where(p => p.Product_Id == id).SingleOrDefault();
            return Json(product, JsonRequestBehavior.AllowGet);
        }



    }
}
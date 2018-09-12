using AMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace AMS.Controllers
{
    public class SaleOrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SaleOrder
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetProducts()
        {
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
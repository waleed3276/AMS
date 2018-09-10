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
    }
}
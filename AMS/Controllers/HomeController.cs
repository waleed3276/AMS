using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public JsonResult GetCompanyName()
        {
            string line = System.IO.File.ReadLines(Server.MapPath("~/Content/AdminInformation.txt")).Skip(1).Take(1).First();
            return Json(line, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCompanyPhone()
        {
            string line = System.IO.File.ReadLines(Server.MapPath("~/Content/AdminInformation.txt")).Skip(4).Take(1).First();
            return Json(line, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCompanyEmail()
        {
            string line = System.IO.File.ReadLines(Server.MapPath("~/Content/AdminInformation.txt")).Skip(7).Take(1).First();
            return Json(line, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCompanyAddress()
        {
            string line = System.IO.File.ReadLines(Server.MapPath("~/Content/AdminInformation.txt")).Skip(10).Take(1).First();
            return Json(line, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNTN()
        {
            string line = System.IO.File.ReadLines(Server.MapPath("~/Content/AdminInformation.txt")).Skip(13).Take(1).First();
            return Json(line, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGST()
        {
            int line = Int32.Parse(System.IO.File.ReadLines(Server.MapPath("~/Content/AdminInformation.txt")).Skip(16).Take(1).First());
            return Json(line, JsonRequestBehavior.AllowGet);
        }
    }
}
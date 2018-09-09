using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AMS.Models;
using Newtonsoft.Json;

namespace AMS.Controllers
{
    public class ProductSizesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ProductSizes
        public ActionResult Index()
        {
            return View(db.ProductSizes.ToList());
        }

        // GET: ProductSizes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductSize productSize = db.ProductSizes.Find(id);
            if (productSize == null)
            {
                return HttpNotFound();
            }
            return View(productSize);
        }

        // GET: ProductSizes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductSizes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductSize_Id,ProductSize_Value,ProductSize_Height,ProductSize_Width,ProductSize_Length,ProductSize_Unit")] ProductSize productSize)
        {
            if (ModelState.IsValid)
            {
                db.ProductSizes.Add(productSize);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(productSize);
        }

        // GET: ProductSizes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductSize productSize = db.ProductSizes.Find(id);
            if (productSize == null)
            {
                return HttpNotFound();
            }
            return View(productSize);
        }

        // POST: ProductSizes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductSize_Id,ProductSize_Value,ProductSize_Height,ProductSize_Width,ProductSize_Length,ProductSize_Unit")] ProductSize productSize)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productSize).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(productSize);
        }

        // GET: ProductSizes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductSize productSize = db.ProductSizes.Find(id);
            if (productSize == null)
            {
                return HttpNotFound();
            }
            return View(productSize);
        }

        // POST: ProductSizes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductSize productSize = db.ProductSizes.Find(id);
            db.ProductSizes.Remove(productSize);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public void CreateProductSize(FormCollection form)
        {
            ProductSize productSize = JsonConvert.DeserializeObject<ProductSize>(form["ProductSizeObj"]);
            decimal length = productSize.ProductSize_Length;
            decimal width = productSize.ProductSize_Width;
            decimal height = productSize.ProductSize_Height;

            if (length > 0 && width > 0 && height > 0)
                productSize.ProductSize_Value = length + "x" + width + "x" + height + "" + productSize.ProductSize_Unit;
            else if (length > 0 && width > 0)
                productSize.ProductSize_Value = length + "x" + width + "" + productSize.ProductSize_Unit;
            else if (length > 0)
                productSize.ProductSize_Value = length + "" + productSize.ProductSize_Unit;
            else
                productSize.ProductSize_Value = "Unknown";

            db.ProductSizes.Add(productSize);
            db.SaveChanges();
        }

        public void UpdateProductSize(FormCollection form)
        {
            ProductSize productSize = JsonConvert.DeserializeObject<ProductSize>(form["ProductSizeObj"]);
            int id = productSize.ProductSize_Id;
            var productSize_db = db.ProductSizes.Find(id);
            decimal length = productSize.ProductSize_Length;
            decimal width = productSize.ProductSize_Width;
            decimal height = productSize.ProductSize_Height;

            productSize_db.ProductSize_Length = length;
            productSize_db.ProductSize_Width = width;
            productSize_db.ProductSize_Height = height;
            productSize_db.ProductSize_Unit = productSize.ProductSize_Unit;

            if (length > 0 && width > 0 && height > 0)
                productSize_db.ProductSize_Value = length + "x" + width + "x" + height + "" + productSize.ProductSize_Unit;
            else if (length > 0 && width > 0)
                productSize_db.ProductSize_Value = length + "x" + width + "" + productSize.ProductSize_Unit;
            else if (length > 0)
                productSize_db.ProductSize_Value = length + "" + productSize.ProductSize_Unit;
            else
                productSize_db.ProductSize_Value = "Unknown";

            db.Entry(productSize_db).State = EntityState.Modified;
            db.SaveChanges();
        }

        public JsonResult GetProductSize()
        {
            var sizes_list = db.ProductSizes.ToList();
            return Json(sizes_list, JsonRequestBehavior.AllowGet);
        }

        public void DeleteProductSize(int id)
        {
            var productSize = db.ProductSizes.Find(id);
            db.ProductSizes.Remove(productSize);
            db.SaveChanges();
        }
    }
}

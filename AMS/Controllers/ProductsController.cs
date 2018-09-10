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
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Products
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.CategorySub).Include(p => p.ProductSize);
            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.CategorySub_Id = new SelectList(db.CategoriesSub, "CategorySub_Id", "CategorySub_Title");
            ViewBag.ProductSize_Id = new SelectList(db.ProductSizes, "ProductSize_Id", "ProductSize_Value");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Product_Id,CategorySub_Id,ProductSize_Id,Product_Title,Product_Code,Product_Description,Product_Weight,Product_Rate,Product_Status,Product_Color,Product_Date,Product_Unit,Product_UnitPrice")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategorySub_Id = new SelectList(db.CategoriesSub, "CategorySub_Id", "CategorySub_Title", product.CategorySub_Id);
            ViewBag.ProductSize_Id = new SelectList(db.ProductSizes, "ProductSize_Id", "ProductSize_Value", product.ProductSize_Id);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategorySub_Id = new SelectList(db.CategoriesSub, "CategorySub_Id", "CategorySub_Title", product.CategorySub_Id);
            ViewBag.ProductSize_Id = new SelectList(db.ProductSizes, "ProductSize_Id", "ProductSize_Value", product.ProductSize_Id);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Product_Id,CategorySub_Id,ProductSize_Id,Product_Title,Product_Code,Product_Description,Product_Weight,Product_Rate,Product_Status,Product_Color,Product_Date,Product_Unit,Product_UnitPrice")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategorySub_Id = new SelectList(db.CategoriesSub, "CategorySub_Id", "CategorySub_Title", product.CategorySub_Id);
            ViewBag.ProductSize_Id = new SelectList(db.ProductSizes, "ProductSize_Id", "ProductSize_Value", product.ProductSize_Id);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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

        public void CreateProduct(FormCollection form)
        {
            Product product = JsonConvert.DeserializeObject<Product>(form["ProductObj"]);
            product.CategorySub = db.CategoriesSub.Find(product.CategorySub_Id);
            product.ProductSize = db.ProductSizes.Find(product.ProductSize_Id);
            product.Product_Date = DateTime.Now;
            product.Product_Status = true;
            db.Products.Add(product);
            db.SaveChanges();
        }

        public void UpdateProduct(FormCollection form)
        {
            Product product = JsonConvert.DeserializeObject<Product>(form["ProductObj"]);
            int id = product.Product_Id;
            var product_db = db.Products.Find(id);
            product_db.CategorySub = db.CategoriesSub.Find(product.CategorySub_Id);
            product_db.ProductSize = db.ProductSizes.Find(product.ProductSize_Id);
            product_db.Product_Title = product.Product_Title;
            product_db.Product_Code = product.Product_Code;
            product_db.Product_Description = product.Product_Description;
            product_db.Product_Color = product.Product_Color;
            product_db.Product_Unit = product.Product_Unit;
            product_db.Product_UnitPrice = product.Product_UnitPrice;
            product_db.Product_Rate = product.Product_Rate;
            db.Entry(product_db).State = EntityState.Modified;
            db.SaveChanges();
        }

        public JsonResult GetProduct()
        {
            var products_list = db.Products.ToList();
            return Json(products_list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCategorySub()
        {
            var catsub_list = db.CategoriesSub.ToList();
            return Json(catsub_list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductSize()
        {
            var sizes_list = db.ProductSizes.ToList();
            return Json(sizes_list, JsonRequestBehavior.AllowGet);
        }

        public void DeleteProduct(int id)
        {
            var product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
        }
    }
}

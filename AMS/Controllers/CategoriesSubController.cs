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
    public class CategoriesSubController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CategoriesSub
        public ActionResult Index()
        {
            var categoriesSub = db.CategoriesSub.Include(c => c.Category);
            return View(categoriesSub.ToList());
        }

        // GET: CategoriesSub/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategorySub categorySub = db.CategoriesSub.Find(id);
            if (categorySub == null)
            {
                return HttpNotFound();
            }
            return View(categorySub);
        }

        // GET: CategoriesSub/Create
        public ActionResult Create()
        {
            ViewBag.Category_Id = new SelectList(db.Categories, "Category_Id", "Category_Title");
            return View();
        }

        // POST: CategoriesSub/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategorySub_Id,Category_Id,CategorySub_Title,CategorySub_Code,CategorySub_Description,CategorySub_Status")] CategorySub categorySub)
        {
            if (ModelState.IsValid)
            {
                db.CategoriesSub.Add(categorySub);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Category_Id = new SelectList(db.Categories, "Category_Id", "Category_Title", categorySub.Category_Id);
            return View(categorySub);
        }

        // GET: CategoriesSub/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategorySub categorySub = db.CategoriesSub.Find(id);
            if (categorySub == null)
            {
                return HttpNotFound();
            }
            ViewBag.Category_Id = new SelectList(db.Categories, "Category_Id", "Category_Title", categorySub.Category_Id);
            return View(categorySub);
        }

        // POST: CategoriesSub/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategorySub_Id,Category_Id,CategorySub_Title,CategorySub_Code,CategorySub_Description,CategorySub_Status")] CategorySub categorySub)
        {
            if (ModelState.IsValid)
            {
                db.Entry(categorySub).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Category_Id = new SelectList(db.Categories, "Category_Id", "Category_Title", categorySub.Category_Id);
            return View(categorySub);
        }

        // GET: CategoriesSub/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategorySub categorySub = db.CategoriesSub.Find(id);
            if (categorySub == null)
            {
                return HttpNotFound();
            }
            return View(categorySub);
        }

        // POST: CategoriesSub/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CategorySub categorySub = db.CategoriesSub.Find(id);
            db.CategoriesSub.Remove(categorySub);
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


        public void CreateCategorySub(FormCollection form)
        {
            CategorySub categorysub = JsonConvert.DeserializeObject<CategorySub>(form["CategorySubObj"]);
            categorysub.CategorySub_Status = true;
            categorysub.Category = db.Categories.Find(categorysub.Category_Id);
            db.CategoriesSub.Add(categorysub);
            db.SaveChanges();
        }

        public void UpdateCategorySub(FormCollection form)
        {
            CategorySub categorysub = JsonConvert.DeserializeObject<CategorySub>(form["CategorySubObj"]);
            var id = categorysub.CategorySub_Id;
            var catsub_db = db.CategoriesSub.Find(id);
            catsub_db.Category_Id = categorysub.Category_Id;
            catsub_db.Category = db.Categories.Find(categorysub.Category_Id);
            catsub_db.CategorySub_Title = categorysub.CategorySub_Title;
            catsub_db.CategorySub_Code = categorysub.CategorySub_Code;
            catsub_db.CategorySub_Description = categorysub.CategorySub_Description;
            db.Entry(catsub_db).State = EntityState.Modified;

            db.SaveChanges();
        }

        public JsonResult GetCategory()
        {
            var cat_list = db.Categories.ToList();
            return Json(cat_list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCategorySub()
        {
            var catsub_list = db.CategoriesSub.ToList();
            return Json(catsub_list, JsonRequestBehavior.AllowGet);
        }

        public void DeleteCategorySub(int id)
        {
            var catsub = db.CategoriesSub.Find(id);
            db.CategoriesSub.Remove(catsub);
            db.SaveChanges();
        }
    }
}

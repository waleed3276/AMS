using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AMS.Models;
using AMS.Models.HardCode;

namespace AMS.Controllers
{
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private DefaultStrings ds = new DefaultStrings();

        // GET: Transactions
        public ActionResult Index()
        {
            var transactions = db.Transactions.Include(t => t.OpeningClosing);
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            ViewBag.OpeningClosing_Id = new SelectList(db.OpeningClosings, "OpeningClosing_Id", "OpeningClosing_Id");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Transaction_Id,OpeningClosing_Id,Transaction_ItemId,Transaction_ItemType,Transaction_Description,Transaction_Debit,Transaction_Credit,Transaction_IsCash,Transaction_BankAccountNo,Transaction_CheckBookNo,Transaction_Date,Transaction_Status")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OpeningClosing_Id = new SelectList(db.OpeningClosings, "OpeningClosing_Id", "OpeningClosing_Id", transaction.OpeningClosing_Id);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.OpeningClosing_Id = new SelectList(db.OpeningClosings, "OpeningClosing_Id", "OpeningClosing_Id", transaction.OpeningClosing_Id);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Transaction_Id,OpeningClosing_Id,Transaction_ItemId,Transaction_ItemType,Transaction_Description,Transaction_Debit,Transaction_Credit,Transaction_IsCash,Transaction_BankAccountNo,Transaction_CheckBookNo,Transaction_Date,Transaction_Status")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OpeningClosing_Id = new SelectList(db.OpeningClosings, "OpeningClosing_Id", "OpeningClosing_Id", transaction.OpeningClosing_Id);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
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

        public ActionResult ViewAll()
        {
            return View(db.Transactions.Where(m => m.Transaction_Status));
        }

        public ActionResult ViewToday()
        {
            var transactions = new List<Transaction>();
            OpeningClosing opencloseObj = new OpeningClosing();
            foreach (var item in db.OpeningClosings)
            {
                if (item.OpeningClosing_Date.ToShortDateString().Equals(DateTime.Now.ToShortDateString()) && !item.OpeningClosing_IsClosed)
                {
                    opencloseObj = item;
                    break;
                }
            }
            foreach (var item in db.Transactions.ToList().Where(m => m.OpeningClosing == opencloseObj).OrderByDescending(m => m.Transaction_Date))
            {
                if (item.Transaction_Status)
                {
                    transactions.Add(item);
                }
            }
            return View(transactions);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(Transaction trans, FormCollection form)
        {
            if (form["isBankAccount"] != null)
            {
                trans.Transaction_BankAccountNo = form["BankAccNo"];
            }
            else if (form["isCheckbook"] != null)
            {
                trans.Transaction_BankAccountNo = form["BankAccNo"];
                trans.Transaction_CheckBookNo = Convert.ToInt32(form["CheckNo"]);
            }
            else if (form["isCash"] != null)
            {
                trans.Transaction_IsCash = true;
            }
            trans.Transaction_Date = DateTime.Now;
            trans.OpeningClosing_Id = db.OpeningClosings.Max(m => m.OpeningClosing_Id);
            trans.OpeningClosing = db.OpeningClosings.Where(m => m.OpeningClosing_Id == trans.OpeningClosing_Id).SingleOrDefault();
            trans.Transaction_ItemId = 0;
            trans.Transaction_ItemType = (form["IsDriverExpense"] == "1") ? ds.Transaction_DriverExpense : ds.Transaction_Manual;
            trans.Transaction_Description = form["Transaction_Description"];
            trans.Transaction_Status = true;
            db.Transactions.Add(trans);
            db.SaveChanges();
            return RedirectToAction("ViewToday");
        }

        public ActionResult Edit2(int? id)
        {
            var trans = db.Transactions.Find(id);
            if (trans.Transaction_Status)
                return View(trans);
            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult Edit(Transaction trans1, FormCollection form)
        {
            Transaction trans = db.Transactions.Find(trans1.Transaction_Id);
            if (form["isBankAccount"] != null)
            {
                trans.Transaction_BankAccountNo = form["BankAccNo"];
            }
            else if (form["isCheckbook"] != null)
            {
                trans.Transaction_BankAccountNo = form["BankAccNo"];
                trans.Transaction_CheckBookNo = Convert.ToInt32(form["CheckNo"]);
            }
            else if (form["isCash"] != null)
            {
                trans.Transaction_IsCash = true;
            }
            trans.Transaction_ItemType = (form["IsDriverExpense"] == "1") ? ds.Transaction_DriverExpense : ds.Transaction_Manual;
            trans.Transaction_Description = trans1.Transaction_Description;
            trans.Transaction_Debit = trans1.Transaction_Debit;
            trans.Transaction_Credit = trans1.Transaction_Credit;
            db.Entry(trans).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ViewToday");
        }

        public ActionResult Delete2(int? id)
        {
            var trans = db.Transactions.Find(id);
            trans.Transaction_Status = false;
            db.Entry(trans).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ViewToday");
        }
    }
}

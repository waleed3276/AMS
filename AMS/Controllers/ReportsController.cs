using AMS.Models;
using AMS.Models.HardCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS.Controllers
{
    public class ReportsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private DefaultStrings ds = new DefaultStrings();

        // GET: Reports
        public ActionResult Index()
        {
            return View(db.Transactions.ToList());
        }

        public ActionResult ViewReport()
        {
            var reportData = TempData["ReportData"];
            ViewBag.MWTReport = TempData["MWTReport"];
            return View(reportData);
        }

        public ActionResult GetReport(int? ReportType, int? Month, int Invoice, int? DateFrom, int? DateTo)
        {
            if (ReportType == 1)
                TempData["MWTReport"] = "Monthly";
            else if (ReportType == 2)
                TempData["MWTReport"] = "Weekly";
            else
                TempData["MWTReport"] = "";

            if (Invoice == 1)
            {
                if (ReportType == 1)
                {
                    if (DateFrom == null || DateTo == null)
                    {
                        var reportData = db.Transactions.Where(r => r.Transaction_Date.Month == Month && r.Transaction_Date.Year == DateTime.Now.Year).ToList();
                        TempData["ReportData"] = reportData;
                    }
                    else
                    {
                        var reportData = db.Transactions.Where(r => r.Transaction_Date.Month == Month && r.Transaction_Date.Day >= DateFrom && r.Transaction_Date.Day <= DateTo && r.Transaction_Date.Year == DateTime.Now.Year).ToList();
                        TempData["ReportData"] = reportData;
                    }

                    return RedirectToAction("ViewReport");
                }
                if (ReportType == 2)
                {
                    var reportData = db.Transactions.Where(r => r.Transaction_Date.Month == Month && r.Transaction_Date.Day >= DateFrom && r.Transaction_Date.Day <= DateTo && r.Transaction_Date.Year == DateTime.Now.Year).ToList();
                    TempData["ReportData"] = reportData;
                    return RedirectToAction("ViewReport");
                }
            }
            else if (Invoice == 2)
            {
                if (ReportType == 1)
                {
                    if (DateFrom == null || DateTo == null)
                    {
                        var reportData = db.Transactions.Where(r => r.Transaction_ItemType == ds.PurchaseInvoiceType && r.Transaction_Date.Month == Month && r.Transaction_Date.Year == DateTime.Now.Year).ToList();
                        TempData["ReportData"] = reportData;
                    }
                    else
                    {
                        var reportData = db.Transactions.Where(r => r.Transaction_ItemType == ds.PurchaseInvoiceType && r.Transaction_Date.Month == Month && r.Transaction_Date.Day >= DateFrom && r.Transaction_Date.Day <= DateTo && r.Transaction_Date.Year == DateTime.Now.Year).ToList();
                        TempData["ReportData"] = reportData;
                    }

                    return RedirectToAction("ViewReport");
                }
                if (ReportType == 2)
                {
                    var reportData = db.Transactions.Where(r => r.Transaction_ItemType == ds.PurchaseInvoiceType && r.Transaction_Date.Month == Month && r.Transaction_Date.Day >= DateFrom && r.Transaction_Date.Day <= DateTo && r.Transaction_Date.Year == DateTime.Now.Year).ToList();
                    TempData["ReportData"] = reportData;
                    return RedirectToAction("ViewReport");
                }
            }
            else if (Invoice == 3)
            {
                if (ReportType == 1)
                {
                    if (DateFrom == null || DateTo == null)
                    {
                        var reportData = db.Transactions.Where(r => r.Transaction_ItemType == ds.SaleInvoiceType && r.Transaction_Date.Month == Month && r.Transaction_Date.Year == DateTime.Now.Year).ToList();
                        TempData["ReportData"] = reportData;
                    }
                    else
                    {
                        var reportData = db.Transactions.Where(r => r.Transaction_ItemType == ds.SaleInvoiceType && r.Transaction_Date.Month == Month && r.Transaction_Date.Day >= DateFrom && r.Transaction_Date.Day <= DateTo && r.Transaction_Date.Year == DateTime.Now.Year).ToList();
                        TempData["ReportData"] = reportData;
                    }

                    return RedirectToAction("ViewReport");
                }
                if (ReportType == 2)
                {
                    var reportData = db.Transactions.Where(r => r.Transaction_ItemType == ds.SaleInvoiceType && r.Transaction_Date.Month == Month && r.Transaction_Date.Day >= DateFrom && r.Transaction_Date.Day <= DateTo && r.Transaction_Date.Year == DateTime.Now.Year).ToList();
                    TempData["ReportData"] = reportData;
                    return RedirectToAction("ViewReport");
                }
            }
            else if (Invoice == 4)
            {
                if (ReportType == 1)
                {
                    if (DateFrom == null || DateTo == null)
                    {
                        var reportData = db.Transactions.Where(r => (r.Transaction_ItemType == ds.Balance_Customer || r.Transaction_ItemType == ds.Balance_Vendor) && r.Transaction_Date.Month == Month && r.Transaction_Date.Year == DateTime.Now.Year).ToList();
                        TempData["ReportData"] = reportData;
                    }
                    else
                    {
                        var reportData = db.Transactions.Where(r => (r.Transaction_ItemType == ds.Balance_Customer || r.Transaction_ItemType == ds.Balance_Vendor) && r.Transaction_Date.Month == Month && r.Transaction_Date.Day >= DateFrom && r.Transaction_Date.Day <= DateTo && r.Transaction_Date.Year == DateTime.Now.Year).ToList();
                        TempData["ReportData"] = reportData;
                    }

                    return RedirectToAction("ViewReport");
                }
                if (ReportType == 2)
                {
                    var reportData = db.Transactions.Where(r => (r.Transaction_ItemType == ds.Balance_Customer || r.Transaction_ItemType == ds.Balance_Vendor) && r.Transaction_Date.Month == Month && r.Transaction_Date.Day >= DateFrom && r.Transaction_Date.Day <= DateTo && r.Transaction_Date.Year == DateTime.Now.Year).ToList();
                    TempData["ReportData"] = reportData;
                    return RedirectToAction("ViewReport");
                }
            }
            return View();
        }

        public ActionResult TodayReport(int Invoice)
        {
            TempData["MWTReport"] = "Today";

            if (Invoice == 1)
            {
                var reportData = db.Transactions.Where(r => r.Transaction_Date.Day == DateTime.Now.Day && r.Transaction_Date.Month == DateTime.Now.Month && r.Transaction_Date.Year == DateTime.Now.Year).ToList();
                TempData["ReportData"] = reportData;
                return RedirectToAction("ViewReport");
            }
            else if (Invoice == 2)
            {
                var reportData = db.Transactions.Where(r => r.Transaction_ItemType == ds.PurchaseInvoiceType && r.Transaction_Date.Day == DateTime.Now.Day && r.Transaction_Date.Month == DateTime.Now.Month && r.Transaction_Date.Year == DateTime.Now.Year).ToList();
                TempData["ReportData"] = reportData;
                return RedirectToAction("ViewReport");
            }
            else if (Invoice == 3)
            {
                var reportData = db.Transactions.Where(r => r.Transaction_ItemType == ds.SaleInvoiceType && r.Transaction_Date.Day == DateTime.Now.Day && r.Transaction_Date.Month == DateTime.Now.Month && r.Transaction_Date.Year == DateTime.Now.Year).ToList();
                TempData["ReportData"] = reportData;
                return RedirectToAction("ViewReport");
            }
            else if (Invoice == 4)
            {
                var reportData = db.Transactions.Where(r => (r.Transaction_ItemType == ds.Balance_Customer || r.Transaction_ItemType == ds.Balance_Vendor) && r.Transaction_Date.Day == DateTime.Now.Day && r.Transaction_Date.Month == DateTime.Now.Month && r.Transaction_Date.Year == DateTime.Now.Year).ToList();
                TempData["ReportData"] = reportData;
                return RedirectToAction("ViewReport");
            }
            else if (Invoice == 5)
            {
                var reportData = db.Transactions.Where(r => (r.Transaction_ItemType == ds.Transaction_DriverExpense || r.Transaction_ItemType == ds.Transaction_Manual) && r.Transaction_Date.Day == DateTime.Now.Day && r.Transaction_Date.Month == DateTime.Now.Month && r.Transaction_Date.Year == DateTime.Now.Year).ToList();
                TempData["ReportData"] = reportData;
                return RedirectToAction("ViewReport");
            }
            return View();
        }
    }
}
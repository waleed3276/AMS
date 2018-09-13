using AMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using AMS.Models.HardCode;

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

        public JsonResult GetSaleOrder()
        {
            if (Session["UserId"] != null)
            {
                string userId = Session["UserId"].ToString();
                var saleOrders = db.SaleOrder_Pts.Where(s => s.Customer.ApplicationUser.Id == userId).ToList();
                return Json(saleOrders, JsonRequestBehavior.AllowGet);
            }
            
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateSaleOrder(FormCollection form)
        {
            if (ModelState.IsValid)
            {
                string userId = Session["UserId"].ToString();
                var js = new JavaScriptSerializer();
                try
                {
                    OrderNumber oNo = new OrderNumber();
                    int soPt_Id = 0;

                    var soPt = JsonConvert.DeserializeObject<SaleOrder_Pt>(form["SaleOrder_PtObj"]);
                    soPt.CustomerId = db.Customers.Where(c => c.ApplicationUser.Id == userId).SingleOrDefault().Customer_Id;
                    soPt.Customer = db.Customers.Where(c => c.ApplicationUser.Id == userId).SingleOrDefault();
                    soPt.SOP_Date = DateTime.Now;
                    soPt.SOP_ModificationDate = DateTime.Now;
                    soPt.SOP_SO = oNo.GenerateSaleOrderNumber().ToString();
                    db.SaleOrder_Pts.Add(soPt);
                    db.SaveChanges();

                    soPt_Id = db.SaleOrder_Pts.Max(s => s.SOP_Id);

                    try
                    {
                        List<SaleOrder_Ch> soCh_list = js.Deserialize<SaleOrder_Ch[]>(form["SaleOrder_ChList"].ToString()).ToList();
                        foreach (var soCh in soCh_list)
                        {
                            soCh.Product = db.Products.Where(p => p.Product_Id == soCh.Product_Id).SingleOrDefault();
                            soCh.SOP_Id = soPt_Id;
                            soCh.SaleOrder_Pt = db.SaleOrder_Pts.Find(soPt_Id);
                            db.SaleOrder_Ches.Add(soCh);
                            db.SaveChanges();
                        }
                    }
                    catch (Exception e)
                    {  }

                    SalePurchaseInvoiceType mSalePurchaseInvoiceType = new SalePurchaseInvoiceType();
                    Invoice invoice = new Invoice()
                    {
                        Invoice_No = mSalePurchaseInvoiceType.GenerateInvoiceNo("SaleOrder"),
                        Invoice_Type = "SaleOrder",
                        SalePurchase_Id = soPt_Id,
                        Invoice_Date = DateTime.Now,
                        Invoice_Status = true
                    };
                    db.Invoices.Add(invoice);
                    db.SaveChanges();

                    return Json("Save", JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {  }
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public void AddTransaction(FormCollection form)
        {
            if (ModelState.IsValid)
            {
                var js = new JavaScriptSerializer();

                int soPt_Id = 0;
                soPt_Id = db.SaleOrder_Pts.Max(s => s.SOP_Id);

                DefaultStrings ds = new DefaultStrings();
                Transaction transaction = js.Deserialize<Transaction>(form["Transaction"]);
                try
                {
                    transaction.Transaction_ItemId = soPt_Id;
                    transaction.Transaction_ItemType = ds.SaleInvoiceType;
                    transaction.Transaction_Date = DateTime.Now;
                    transaction.OpeningClosing_Id = db.OpeningClosings.Max(o => o.OpeningClosing_Id);
                    transaction.OpeningClosing = db.OpeningClosings.Where(o => o.OpeningClosing_Id == transaction.OpeningClosing_Id).SingleOrDefault();
                    transaction.Transaction_Status = true;
                    if (form["isBankAccount"] == "true")
                    {
                        transaction.Transaction_IsCash = false;
                        transaction.Transaction_CheckBookNo = 0;
                        transaction.Transaction_BankAccountNo = form["BankAccountNo"];
                    }
                    else if (form["isCheckbook"] == "true")
                    {
                        transaction.Transaction_IsCash = false;
                        transaction.Transaction_CheckBookNo = Convert.ToInt32(form["CheckNo"]);
                        transaction.Transaction_BankAccountNo = form["BankAccountNo"];
                    }
                    else if (form["isCash"] == "true")
                    {
                        transaction.Transaction_IsCash = true;
                        transaction.Transaction_CheckBookNo = 0;
                        transaction.Transaction_BankAccountNo = "";
                    }
                    db.Transactions.Add(transaction);
                    db.SaveChanges();
                }
                catch (Exception e)
                {  }
            }
        }
    }
}
using AMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using AMS.Models.HardCode;
using System.Data.Entity;

namespace AMS.Controllers
{
    public class SaleOrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private DefaultStrings ds = new DefaultStrings();

        // GET: SaleOrder
        public ActionResult Index()
        {
            return View();
        }

        // GET: SaleOrder/Create
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult SaleOrderInvoice(int? id, string In = null, string docNo = null)
        {
            ViewBag.In = In;
            ViewBag.docNo = docNo;
            if (id != null)
            {
                var saleOrder = db.SaleOrder_Pts.Where(s => s.SOP_Id == id).SingleOrDefault();
                if (saleOrder != null)
                    ViewBag.Date = saleOrder.SOP_ModificationDate;
                else
                    ViewBag.Date = DateTime.Now;
                return View(saleOrder);
            }
            return View();
        }

        public JsonResult CheckSaleOrderNoExist(string soNumber)
        {
            if (soNumber != "")
            {
                var obj = db.SaleOrder_Pts.Where(s => s.SOP_SO == soNumber).SingleOrDefault();
                if (obj != null)
                    return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomers()
        {
            if (User.IsInRole(ds.Role_Admin))
            {
                var customer_list = db.Customers.ToList();
                return Json(customer_list, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
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
                
                List<Tuple<SaleOrder_Pt, decimal, string, int, int, string>> obj = new List<Tuple<SaleOrder_Pt, decimal, string, int, int, string>>();
                foreach (var item in saleOrders)
                {
                    try
                    {
                        var trans = db.Transactions.Where(m => m.Transaction_Status && m.Transaction_ItemId == item.SOP_Id && m.Transaction_ItemType == ds.SaleInvoiceType).SingleOrDefault();

                        int action = 0;
                        if (DateTime.Now.ToShortDateString() == item.SOP_ModificationDate.ToShortDateString() && item.SOP_Status == ds.Status_Pending)
                        {
                            action = 1;
                        }
                        
                        int invoice_no = db.Invoices.Where(m => m.SalePurchase_Id == item.SOP_Id && m.Invoice_Type == ds.SaleInvoiceType).SingleOrDefault().Invoice_No;
                        obj.Add(new Tuple<SaleOrder_Pt, decimal, string, int, int, string>(item, item.SOP_TotalAmount - item.SOP_TotalReceived, trans.Transaction_Description, invoice_no, action, item.SOP_ModificationDate.ToShortDateString()));

                    }
                    catch (Exception ex)
                    {

                    }

                }
                return Json(obj, JsonRequestBehavior.AllowGet);
                //return Json(saleOrders, JsonRequestBehavior.AllowGet);
            }
            
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateSaleOrder(FormCollection form)
        {
            string userId = Session["UserId"].ToString();
            if (ModelState.IsValid)
            {
                var js = new JavaScriptSerializer();
                try
                {
                    //OrderNumber oNo = new OrderNumber();
                    int soPt_Id = 0;

                    var soPt = JsonConvert.DeserializeObject<SaleOrder_Pt>(form["SaleOrder_PtObj"]);
                    if (User.IsInRole(ds.Role_Customer))
                    {
                        soPt.CustomerId = db.Customers.Where(c => c.ApplicationUser.Id == userId).SingleOrDefault().Customer_Id;
                        soPt.Customer = db.Customers.Where(c => c.ApplicationUser.Id == userId).SingleOrDefault();
                    }
                    else if (User.IsInRole(ds.Role_Admin) && soPt.CustomerId > 0)
                    {
                        soPt.Customer = db.Customers.Where(c => c.Customer_Id == soPt.CustomerId).SingleOrDefault();
                    }
                    else
                    {
                        return Json("Not Saved", JsonRequestBehavior.AllowGet);
                    }

                    soPt.SOP_Date = DateTime.Now;
                    soPt.SOP_ModificationDate = DateTime.Now;
                    soPt.SOP_DeliveryDate = Convert.ToDateTime(js.Deserialize<string>(form["DeliveryDate"]));
                    soPt.SOP_Status = ds.Status_Pending;
                    //soPt.SOP_SO = oNo.GenerateSaleOrderNumber().ToString();
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
                        //Invoice_No = mSalePurchaseInvoiceType.GenerateInvoiceNo(ds.SaleInvoiceType),
                        Invoice_No = js.Deserialize<int>(form["InvoiceNo"]),
                        Invoice_DocumentNo = js.Deserialize<int>(form["Invoice_DocumentNo"]),
                        Invoice_Type = ds.SaleInvoiceType,
                        SalePurchase_Id = soPt_Id,
                        Invoice_Date = DateTime.Now,
                        Invoice_Status = true
                    };
                    db.Invoices.Add(invoice);
                    db.SaveChanges();

                    Notification noti = new Notification();
                    noti.Notification_Detail = "New Sale Order of SO #: '" + soPt.SOP_SO + "' has been created by customer.";
                    if (User.IsInRole(ds.Role_Customer))
                    {
                        noti.Id = userId;
                        noti.ApplicationUser = db.Users.Where(u => u.Id == userId).SingleOrDefault();
                    }
                    else
                    {
                        noti.Id = soPt.Customer.ApplicationUser.Id;
                        noti.ApplicationUser = soPt.Customer.ApplicationUser;
                    }
                    noti.Notification_ItemId = soPt_Id;
                    noti.Notification_ItemType = ds.Role_Admin;
                    noti.Notification_Date = DateTime.Now;
                    noti.Notification_IsSeen = false;
                    noti.Notification_Status = true;
                    db.Notifications.Add(noti);
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

        public void AddCustomerRemaining(FormCollection form)
        {
            if (Session["UserId"] != null)
            {
                var js = new JavaScriptSerializer();
                int RemainingAmount = Convert.ToInt32(js.Deserialize<float>(form["RemainingAmount"]));

                string userId = Session["UserId"].ToString();
                var customer = db.Customers.Where(s => s.ApplicationUser.Id == userId).SingleOrDefault();
                customer.Customer_Remaining += RemainingAmount;
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public JsonResult UpdateSaleOrder(FormCollection form)
        {
            string userId = Session["UserId"].ToString();
            if (ModelState.IsValid && userId != null)
            {
                var js = new JavaScriptSerializer();
                try
                {
                    var soPt = JsonConvert.DeserializeObject<SaleOrder_Pt>(form["SaleOrder_PtObj"]);
                    var soPt_db = db.SaleOrder_Pts.Where(s => s.SOP_Id == soPt.SOP_Id).SingleOrDefault();
                    soPt_db.SOP_TotalQuantity = soPt.SOP_TotalQuantity;
                    soPt_db.SOP_TotalAmount = soPt.SOP_TotalAmount;
                    soPt_db.SOP_TotalReceived = soPt.SOP_TotalReceived;
                    soPt_db.SOP_ModificationDate = DateTime.Now;
                    //soPt_db.SOP_GST = soPt.SOP_GST;
                    soPt_db.SOP_SO = soPt.SOP_SO;
                    db.Entry(soPt_db).State = EntityState.Modified;
                    db.SaveChanges();

                    try
                    {
                        List<SaleOrder_Ch> soCh_list = js.Deserialize<SaleOrder_Ch[]>(form["SaleOrder_ChList"].ToString()).ToList();
                        foreach (var soCh in soCh_list)
                        {
                            if (soCh.SOC_Id > 0)
                            {
                                var soCh_db = db.SaleOrder_Ches.Where(s => s.SOC_Id == soCh.SOC_Id).SingleOrDefault();
                                soCh_db.Product = db.Products.Where(p => p.Product_Id == soCh.Product_Id).SingleOrDefault();
                                soCh_db.SOC_Quantity = soCh.SOC_Quantity;
                                soCh_db.SOC_Rate = soCh.SOC_Rate;
                                soCh_db.SOC_Description = soCh.SOC_Description;
                                soCh_db.SOC_Amount = soCh.SOC_Amount;
                                soCh_db.SOC_ItemCode = soCh.SOC_ItemCode;
                                soCh_db.SOC_Unit = soCh.SOC_Unit;
                                db.Entry(soCh_db).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                soCh.Product = db.Products.Where(p => p.Product_Id == soCh.Product_Id).SingleOrDefault();
                                soCh.SOP_Id = soPt.SOP_Id;
                                soCh.SaleOrder_Pt = db.SaleOrder_Pts.Find(soPt.SOP_Id);
                                db.SaleOrder_Ches.Add(soCh);
                                db.SaveChanges();
                            }
                        }

                        List<SaleOrder_Ch> deleted_SaleOrder_ChList = js.Deserialize<SaleOrder_Ch[]>(form["Deleted_SaleOrder_ChList"].ToString()).ToList();
                        foreach (var item in deleted_SaleOrder_ChList)
                        {
                            if (item.SOC_Id > 0)
                            {
                                var socDel = db.SaleOrder_Ches.Find(item.SOC_Id);
                                db.SaleOrder_Ches.Remove(socDel);
                                db.SaveChanges();
                            }
                        }
                    }
                    catch (Exception e)
                    { }

                    Notification noti = new Notification();
                    noti.Notification_Detail = "Sale Order of SO #: '" + soPt.SOP_SO + "' has been updated by customer.";
                    noti.Id = userId;
                    noti.ApplicationUser = db.Users.Where(u => u.Id == userId).SingleOrDefault();
                    noti.Notification_ItemId = soPt.SOP_Id;
                    noti.Notification_ItemType = ds.Role_Admin;
                    noti.Notification_Date = DateTime.Now;
                    noti.Notification_IsSeen = false;
                    noti.Notification_Status = true;
                    db.Notifications.Add(noti);
                    db.SaveChanges();

                    return Json("Update", JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                { }
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public void UpdateTransaction(FormCollection form)
        {
            if (ModelState.IsValid)
            {
                var js = new JavaScriptSerializer();

                int soPt_Id = js.Deserialize<int>(form["SOP_Id"]);

                Transaction transaction = js.Deserialize<Transaction>(form["Transaction"]);
                int transId = db.Transactions.Where(t => t.Transaction_ItemId == soPt_Id && t.Transaction_ItemType == ds.SaleInvoiceType).Max(t => t.Transaction_Id);
                Transaction trans_db = db.Transactions.Where(t => t.Transaction_Id == transId).SingleOrDefault();
                try
                {
                    trans_db.Transaction_Debit = transaction.Transaction_Debit;
                    trans_db.Transaction_Description = transaction.Transaction_Description;
                    trans_db.OpeningClosing_Id = db.OpeningClosings.Max(o => o.OpeningClosing_Id);
                    trans_db.OpeningClosing = db.OpeningClosings.Where(o => o.OpeningClosing_Id == trans_db.OpeningClosing_Id).SingleOrDefault();
                    trans_db.Transaction_Status = true;
                    if (form["isBankAccount"] == "true")
                    {
                        trans_db.Transaction_IsCash = false;
                        trans_db.Transaction_CheckBookNo = 0;
                        trans_db.Transaction_BankAccountNo = form["BankAccountNo"];
                    }
                    else if (form["isCheckbook"] == "true")
                    {
                        trans_db.Transaction_IsCash = false;
                        trans_db.Transaction_CheckBookNo = Convert.ToInt32(form["CheckNo"]);
                        trans_db.Transaction_BankAccountNo = form["BankAccountNo"];
                    }
                    else if (form["isCash"] == "true")
                    {
                        trans_db.Transaction_IsCash = true;
                        trans_db.Transaction_CheckBookNo = 0;
                        trans_db.Transaction_BankAccountNo = "";
                    }
                    db.Entry(trans_db).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception e)
                { }
            }
        }
        
        public void UpdateCustomerRemaining(FormCollection form)
        {
            if (Session["UserId"] != null)
            {
                var js = new JavaScriptSerializer();
                int RemainingAmount = Convert.ToInt32(js.Deserialize<float>(form["RemainingAmount"]));
                int RemainingOld = Convert.ToInt32(js.Deserialize<float>(form["RemainingAmountOld"]));

                string userId = Session["UserId"].ToString();
                var customer = db.Customers.Where(s => s.ApplicationUser.Id == userId).SingleOrDefault();
                customer.Customer_Remaining -= RemainingOld;
                customer.Customer_Remaining += RemainingAmount;
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public JsonResult LoadSaleOrder(int SoPtId)
        {
            var saleOrderCh_List = db.SaleOrder_Ches.Where(s => s.SOP_Id == SoPtId).ToList();
            return Json(saleOrderCh_List, JsonRequestBehavior.AllowGet);
        }

        public void DeleteSaleOrder(int SoPtId)
        {
            var saleOrderPt = db.SaleOrder_Pts.Where(s => s.SOP_Id == SoPtId).SingleOrDefault();
            saleOrderPt.SOP_Status = ds.Status_Cancel;
            db.Entry(saleOrderPt).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
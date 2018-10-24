using AMS.Models;
using AMS.Models.HardCode;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace AMS.Controllers
{
    public class PurchaseOrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private DefaultStrings ds = new DefaultStrings();

        // GET: PurchaseOrder
        public ActionResult Index()
        {
            return View();
        }

        // GET: PurchaseOrder/Create
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult PurchaseOrderInvoice(int? id, string In = null, string docNo = null)
        {
            ViewBag.In = In;
            ViewBag.docNo = docNo;
            if (id != null)
            {
                var purchaseOrder = db.PurchaseOrder_Pts.Where(s => s.POP_Id == id).SingleOrDefault();
                if (purchaseOrder != null)
                    ViewBag.Date = purchaseOrder.POP_ModificationDate;
                else
                    ViewBag.Date = DateTime.Now;
                return View(purchaseOrder);
            }
            return View();
        }

        public JsonResult GetVendors()
        {
            var vendor_list = db.Vendors.ToList();
            return Json(vendor_list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSaleOrder()
        {
            var saleOrders = db.SaleOrder_Pts.Where(s => s.SOP_Status == ds.Status_Pending || s.SOP_Status == ds.Status_InProcess).ToList();

            List<Tuple<SaleOrder_Pt, decimal, string, string>> obj = new List<Tuple<SaleOrder_Pt, decimal, string, string>>();
            foreach (var item in saleOrders)
            {
                try
                {
                    var trans = db.Transactions.Where(m => m.Transaction_Status && m.Transaction_ItemId == item.SOP_Id && m.Transaction_ItemType == ds.SaleInvoiceType).SingleOrDefault();
                    obj.Add(new Tuple<SaleOrder_Pt, decimal, string, string>(item, item.SOP_TotalAmount - item.SOP_TotalReceived, trans.Transaction_Description, item.SOP_ModificationDate.ToShortDateString()));
                }
                catch (Exception ex)
                {  }
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public void SaleOrderApprove(int soId)
        {
            var soPtObj = db.SaleOrder_Pts.Find(soId);
            soPtObj.SOP_Status = ds.Status_Approve;
            db.Entry(soPtObj).State = EntityState.Modified;
            db.SaveChanges();

            Notification noti = new Notification();
            noti.Notification_Detail = "Admin has approved your sale order of SO #: '" + soPtObj.SOP_SO + "'.";
            noti.Id = soPtObj.Customer.ApplicationUser.Id;
            noti.ApplicationUser = soPtObj.Customer.ApplicationUser;
            noti.Notification_ItemId = soId;
            noti.Notification_ItemType = ds.Role_Customer;
            noti.Notification_Date = DateTime.Now;
            noti.Notification_IsSeen = false;
            noti.Notification_Status = true;
            db.Notifications.Add(noti);
            db.SaveChanges();
        }

        public JsonResult StatusChangeSO_FillPOC(int soId)
        {
            var soPtObj = db.SaleOrder_Pts.Find(soId);
            if (soPtObj.SOP_Status == ds.Status_Pending)
            {
                soPtObj.SOP_Status = ds.Status_InProcess;
                db.Entry(soPtObj).State = EntityState.Modified;
                db.SaveChanges();
            }

            var soCh_list = db.SaleOrder_Ches.Where(s => s.SOP_Id == soId).ToList();
            return Json(soCh_list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPurchaseOrder()
        {
            var purchaseOrders = db.PurchaseOrder_Pts.ToList();

            List<Tuple<PurchaseOrder_Pt, decimal, string, int, int, string>> obj = new List<Tuple<PurchaseOrder_Pt, decimal, string, int, int, string>>();
            foreach (var item in purchaseOrders)
            {
                try
                {
                    var trans = db.Transactions.Where(m => m.Transaction_Status && m.Transaction_ItemId == item.POP_Id && m.Transaction_ItemType == ds.PurchaseInvoiceType).SingleOrDefault();

                    int action = 0;
                    if (DateTime.Now.ToShortDateString() == item.POP_ModificationDate.ToShortDateString() && item.POP_Status == ds.Status_Pending)
                    {
                        action = 1;
                    }

                    int invoice_no = db.Invoices.Where(m => m.SalePurchase_Id == item.POP_Id && m.Invoice_Type == ds.PurchaseInvoiceType).SingleOrDefault().Invoice_No;
                    obj.Add(new Tuple<PurchaseOrder_Pt, decimal, string, int, int, string>(item, item.POP_TotalAmount - item.POP_TotalPaid, trans.Transaction_Description, invoice_no, action, item.POP_ModificationDate.ToShortDateString()));

                }
                catch (Exception ex)
                {

                }

            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreatePurchaseOrder(FormCollection form)
        {
            if (ModelState.IsValid)
            {
                var js = new JavaScriptSerializer();
                try
                {
                    int poPt_Id = 0;

                    var poPt = JsonConvert.DeserializeObject<PurchaseOrder_Pt>(form["PurchaseOrder_PtObj"]);
                    poPt.Vendor = db.Vendors.Where(v => v.Vendor_Id == poPt.Vendor_Id).SingleOrDefault();
                    poPt.POP_Date = DateTime.Now;
                    poPt.POP_ModificationDate = DateTime.Now;
                    poPt.POP_DeliveryDate = Convert.ToDateTime(js.Deserialize<string>(form["DeliveryDate"]));
                    poPt.POP_Status = ds.Status_Pending;
                    db.PurchaseOrder_Pts.Add(poPt);
                    db.SaveChanges();

                    poPt_Id = db.PurchaseOrder_Pts.Max(s => s.POP_Id);

                    try
                    {
                        List<PurchaseOrder_Ch> poCh_list = js.Deserialize<PurchaseOrder_Ch[]>(form["PurchaseOrder_ChList"].ToString()).ToList();
                        foreach (var poCh in poCh_list)
                        {
                            poCh.Product = db.Products.Where(p => p.Product_Id == poCh.Product_Id).SingleOrDefault();
                            poCh.POP_Id = poPt_Id;
                            poCh.PurchaseOrder_Pt = db.PurchaseOrder_Pts.Find(poPt_Id);
                            db.PurchaseOrder_Ches.Add(poCh);
                            db.SaveChanges();
                        }
                    }
                    catch (Exception e)
                    { }

                    SalePurchaseInvoiceType mSalePurchaseInvoiceType = new SalePurchaseInvoiceType();
                    Invoice invoice = new Invoice()
                    {
                        //Invoice_No = mSalePurchaseInvoiceType.GenerateInvoiceNo(ds.PurchaseInvoiceType),
                        Invoice_No = js.Deserialize<int>(form["InvoiceNo"]),
                        Invoice_DocumentNo = js.Deserialize<int>(form["Invoice_DocumentNo"]),
                        Invoice_Type = ds.PurchaseInvoiceType,
                        SalePurchase_Id = poPt_Id,
                        Invoice_Date = DateTime.Now,
                        Invoice_Status = true
                    };
                    db.Invoices.Add(invoice);
                    db.SaveChanges();

                    var user = db.Users.Where(u => u.Id == poPt.Vendor.Id).SingleOrDefault();
                    Notification noti = new Notification();
                    noti.Notification_Detail = "New Purchase Order of PO #: '" + poPt.POP_PO + "' has been created by admin.";
                    noti.Id = user.Id;
                    noti.ApplicationUser = user;
                    noti.Notification_ItemId = poPt_Id;
                    noti.Notification_ItemType = ds.Role_Vendor;
                    noti.Notification_Date = DateTime.Now;
                    noti.Notification_IsSeen = false;
                    noti.Notification_Status = true;
                    db.Notifications.Add(noti);
                    db.SaveChanges();

                    return Json("Save", JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                { }
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public void AddTransaction(FormCollection form)
        {
            if (ModelState.IsValid)
            {
                var js = new JavaScriptSerializer();

                int poPt_Id = 0;
                poPt_Id = db.PurchaseOrder_Pts.Max(s => s.POP_Id);

                Transaction transaction = js.Deserialize<Transaction>(form["Transaction"]);
                try
                {
                    transaction.Transaction_ItemId = poPt_Id;
                    transaction.Transaction_ItemType = ds.PurchaseInvoiceType;
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
                { }
            }
        }

        public void AddVendorRemaining(FormCollection form)
        {
            var js = new JavaScriptSerializer();
            int RemainingAmount = Convert.ToInt32(js.Deserialize<float>(form["RemainingAmount"]));

            int vendor_Id = js.Deserialize<int>(form["Vendor_Id"]);
            var vendor = db.Vendors.Where(s => s.Vendor_Id == vendor_Id).SingleOrDefault();
            vendor.Vendor_Remaining += RemainingAmount;
            db.Entry(vendor).State = EntityState.Modified;
            db.SaveChanges();
        }

        public JsonResult UpdatePurchaseOrder(FormCollection form)
        {
            if (ModelState.IsValid)
            {
                var js = new JavaScriptSerializer();
                try
                {
                    var poPt = JsonConvert.DeserializeObject<PurchaseOrder_Pt>(form["PurchaseOrder_PtObj"]);
                    var poPt_db = db.PurchaseOrder_Pts.Where(s => s.POP_Id == poPt.POP_Id).SingleOrDefault();
                    poPt_db.POP_TotalQuantity = poPt.POP_TotalQuantity;
                    poPt_db.POP_TotalAmount = poPt.POP_TotalAmount;
                    poPt_db.POP_TotalPaid = poPt.POP_TotalPaid;
                    poPt_db.POP_ModificationDate = DateTime.Now;
                    //poPt_db.POP_GST = poPt.POP_GST;
                    poPt_db.POP_PO = poPt.POP_PO;
                    db.Entry(poPt_db).State = EntityState.Modified;
                    db.SaveChanges();

                    try
                    {
                        List<PurchaseOrder_Ch> poCh_list = js.Deserialize<PurchaseOrder_Ch[]>(form["PurchaseOrder_ChList"].ToString()).ToList();
                        foreach (var poCh in poCh_list)
                        {
                            if (poCh.POC_Id > 0)
                            {
                                var poCh_db = db.PurchaseOrder_Ches.Where(s => s.POC_Id == poCh.POC_Id).SingleOrDefault();
                                poCh_db.Product = db.Products.Where(p => p.Product_Id == poCh.Product_Id).SingleOrDefault();
                                poCh_db.POC_Quantity = poCh.POC_Quantity;
                                poCh_db.POC_Rate = poCh.POC_Rate;
                                poCh_db.POC_Description = poCh.POC_Description;
                                poCh_db.POC_Amount = poCh.POC_Amount;
                                poCh_db.POC_ItemCode = poCh.POC_ItemCode;
                                poCh_db.POC_Unit = poCh.POC_Unit;
                                db.Entry(poCh_db).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                poCh.Product = db.Products.Where(p => p.Product_Id == poCh.Product_Id).SingleOrDefault();
                                poCh.POP_Id = poPt.POP_Id;
                                poCh.PurchaseOrder_Pt = db.PurchaseOrder_Pts.Find(poPt.POP_Id);
                                db.PurchaseOrder_Ches.Add(poCh);
                                db.SaveChanges();
                            }
                        }

                        List<PurchaseOrder_Ch> deleted_PurchaseOrder_ChList = js.Deserialize<PurchaseOrder_Ch[]>(form["Deleted_PurchaseOrder_ChList"].ToString()).ToList();
                        foreach (var item in deleted_PurchaseOrder_ChList)
                        {
                            if (item.POC_Id > 0)
                            {
                                var socDel = db.PurchaseOrder_Ches.Find(item.POC_Id);
                                db.PurchaseOrder_Ches.Remove(socDel);
                                db.SaveChanges();
                            }
                        }
                    }
                    catch (Exception e)
                    { }

                    var user = db.Users.Where(u => u.Id == poPt.Vendor.Id).SingleOrDefault();
                    Notification noti = new Notification();
                    noti.Notification_Detail = "Purchase Order of PO #: '" + poPt.POP_PO + "' has been updated by admin.";
                    noti.Id = user.Id;
                    noti.ApplicationUser = user;
                    noti.Notification_ItemId = poPt.POP_Id;
                    noti.Notification_ItemType = ds.Role_Vendor;
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

                int poPt_Id = js.Deserialize<int>(form["POP_Id"]);

                Transaction transaction = js.Deserialize<Transaction>(form["Transaction"]);
                int transId = db.Transactions.Where(t => t.Transaction_ItemId == poPt_Id && t.Transaction_ItemType == ds.PurchaseInvoiceType).Max(t => t.Transaction_Id);
                Transaction trans_db = db.Transactions.Where(t => t.Transaction_Id == transId).SingleOrDefault();
                try
                {
                    trans_db.Transaction_Credit = transaction.Transaction_Credit;
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

        public void UpdateVendorRemaining(FormCollection form)
        {
            var js = new JavaScriptSerializer();
            int RemainingAmount = Convert.ToInt32(js.Deserialize<float>(form["RemainingAmount"]));
            int RemainingOld = Convert.ToInt32(js.Deserialize<float>(form["RemainingAmountOld"]));

            int vendor_Id = js.Deserialize<int>(form["Vendor_Id"]);
            var vendor = db.Vendors.Where(s => s.Vendor_Id == vendor_Id).SingleOrDefault();
            vendor.Vendor_Remaining -= RemainingOld;
            vendor.Vendor_Remaining += RemainingAmount;
            db.Entry(vendor).State = EntityState.Modified;
            db.SaveChanges();
        }

        public JsonResult LoadPurchaseOrder(int PoPtId)
        {
            var purchaseOrderCh_List = db.PurchaseOrder_Ches.Where(s => s.POP_Id == PoPtId).ToList();
            return Json(purchaseOrderCh_List, JsonRequestBehavior.AllowGet);
        }

        public void DeletePurchaseOrder(int PoPtId)
        {
            var purchaseOrderPt = db.PurchaseOrder_Pts.Where(s => s.POP_Id == PoPtId).SingleOrDefault();
            purchaseOrderPt.POP_Status = ds.Status_Cancel;
            db.Entry(purchaseOrderPt).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
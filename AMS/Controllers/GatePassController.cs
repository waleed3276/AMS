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
    public class GatePassController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private DefaultStrings ds = new DefaultStrings();

        // GET: GatePass
        public ActionResult Index()
        {
            return View();
        }

        // GET: GatePass/Create
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult GPInvoice(int? id, string gp = null)
        {
            ViewBag.gpNo = gp;
            if (id != null)
            {
                var gatepass = db.GatePasses.Where(s => s.PurchaseOrder_Pt.POP_Id == id).SingleOrDefault();
                if (gatepass != null)
                    ViewBag.Date = gatepass.GatePass_Date;
                else
                    ViewBag.Date = DateTime.Now;
                return View(gatepass);
            }
            return View();
        }

        public JsonResult GetCustomerToShip(int poId)
        {
            string poNumber = db.PurchaseOrder_Pts.Where(p => p.POP_Id == poId).SingleOrDefault().POP_PO;
            var customer = db.SaleOrder_Pts.Where(s => s.SOP_SO == poNumber).SingleOrDefault().Customer;
            return Json(customer, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPurchaseOrder()
        {
            var purchaseOrders = db.PurchaseOrder_Pts.Where(s => s.POP_Status == ds.Status_Pending || s.POP_Status == ds.Status_InProcess).ToList();

            List<Tuple<PurchaseOrder_Pt, decimal, string, string>> obj = new List<Tuple<PurchaseOrder_Pt, decimal, string, string>>();
            foreach (var item in purchaseOrders)
            {
                try
                {
                    var trans = db.Transactions.Where(m => m.Transaction_Status && m.Transaction_ItemId == item.POP_Id && m.Transaction_ItemType == ds.PurchaseInvoiceType).SingleOrDefault();
                    obj.Add(new Tuple<PurchaseOrder_Pt, decimal, string, string>(item, item.POP_TotalAmount - item.POP_TotalPaid, trans.Transaction_Description, item.POP_ModificationDate.ToShortDateString()));
                }
                catch (Exception ex)
                { }
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public void PurchaseOrderApprove(int poId)
        {
            var poPtObj = db.PurchaseOrder_Pts.Find(poId);
            poPtObj.POP_Status = ds.Status_Approve;
            db.Entry(poPtObj).State = EntityState.Modified;
            db.SaveChanges();

            Notification noti = new Notification();
            noti.Notification_Detail = "Vendor has approved your purchase order of PO #: '" + poPtObj.POP_PO + "'.";
            noti.Id = poPtObj.Vendor.ApplicationUser.Id;
            noti.ApplicationUser = poPtObj.Vendor.ApplicationUser;
            noti.Notification_ItemId = poId;
            noti.Notification_ItemType = ds.Role_Admin;
            noti.Notification_Date = DateTime.Now;
            noti.Notification_IsSeen = false;
            noti.Notification_Status = true;
            db.Notifications.Add(noti);
            db.SaveChanges();
        }

        public JsonResult StatusChangePO_FillDC(int poId)
        {
            var poPtObj = db.PurchaseOrder_Pts.Find(poId);
            if (poPtObj.POP_Status == ds.Status_Pending)
            {
                poPtObj.POP_Status = ds.Status_InProcess;
                db.Entry(poPtObj).State = EntityState.Modified;
                db.SaveChanges();
            }

            var poCh_list = db.PurchaseOrder_Ches.Where(s => s.POP_Id == poId).ToList();
            return Json(poCh_list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGatePass()
        {
            var gatePasses = db.GatePasses.ToList();

            List<Tuple<PurchaseOrder_Pt, decimal, string, int, string, string>> obj = new List<Tuple<PurchaseOrder_Pt, decimal, string, int, string, string>>();
            foreach (var item in gatePasses)
            {
                try
                {
                    var trans = db.Transactions.Where(m => m.Transaction_Status && m.Transaction_ItemId == item.PurchaseOrder_Pt.POP_Id && m.Transaction_ItemType == ds.PurchaseInvoiceType).SingleOrDefault();
                    int invoice_no = db.Invoices.Where(m => m.SalePurchase_Id == item.PurchaseOrder_Pt.POP_Id && m.Invoice_Type == ds.PurchaseInvoiceType).SingleOrDefault().Invoice_No;
                    obj.Add(new Tuple<PurchaseOrder_Pt, decimal, string, int, string, string>(item.PurchaseOrder_Pt, item.PurchaseOrder_Pt.POP_TotalAmount - item.PurchaseOrder_Pt.POP_TotalPaid, trans.Transaction_Description, invoice_no, item.GatePass_Date.ToShortDateString(), item.GatePass_No));

                }
                catch (Exception ex)
                {

                }

            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GenerateGatePass(FormCollection form)
        {
            if (ModelState.IsValid)
            {
                var js = new JavaScriptSerializer();
                OrderNumber on = new OrderNumber();
                try
                {
                    int poPtId = JsonConvert.DeserializeObject<int>(form["poPtId"]);
                    var gatePass = JsonConvert.DeserializeObject<GatePass>(form["GatePassObj"]);
                    gatePass.PurchaseOrder_PtId = poPtId;
                    gatePass.PurchaseOrder_Pt = db.PurchaseOrder_Pts.Where(p => p.POP_Id == poPtId).SingleOrDefault();
                    string poNumber = gatePass.PurchaseOrder_Pt.POP_PO;
                    gatePass.Customer_Id = db.SaleOrder_Pts.Where(s => s.SOP_SO == poNumber).SingleOrDefault().CustomerId;
                    gatePass.Customer = db.Customers.Where(s => s.Customer_Id == gatePass.Customer_Id).SingleOrDefault();
                    gatePass.GatePass_No = on.GenerateGatePassNumber().ToString();
                    gatePass.GatePass_Date = DateTime.Now;
                    gatePass.GatePass_Status = true;
                    db.GatePasses.Add(gatePass);
                    db.SaveChanges();

                    return Json("Save", JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                { }
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}
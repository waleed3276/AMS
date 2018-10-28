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
    public class SalesTaxController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private DefaultStrings ds = new DefaultStrings();

        // GET: SalesTax
        public ActionResult Index()
        {
            return View();
        }

        // GET: SalesTax/Create
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult SalesTaxInvoice(int? id, string In = null, string DC = null)
        {
            ViewBag.In = In;
            ViewBag.DC = DC;
            if (id != null)
            {
                var saleTax = db.SalesTax_Pts.Where(s => s.STP_Id == id).SingleOrDefault();
                if (saleTax != null)
                    ViewBag.Date = saleTax.STP_Date;
                else
                    ViewBag.Date = DateTime.Now;
                return View(saleTax);
            }
            return View();
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

        public JsonResult GetCustomerSOList(int? id)
        {
            if (id > 0)
            {
                var dataList = db.SaleOrder_Pts.Where(s => s.CustomerId == id).ToList();
                foreach (var data in dataList)
                {
                    var subdatalist = db.SaleOrder_Ches.Where(s => s.SaleOrder_Pt.SOP_Id == data.SOP_Id && s.SOC_SalesTaxStatus == ds.Status_Pending).ToList();
                    if (subdatalist.Count == 0)
                        dataList.Remove(data);
                }
                return Json(dataList, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerProductList(int id, int soPtId)
        {
            if (id > 0 && soPtId > 0)
            {
                var soCh = db.SaleOrder_Ches.Where(s => s.SaleOrder_Pt.SOP_Id == soPtId && s.SOC_SalesTaxStatus == ds.Status_Pending).ToList();
                return Json(soCh, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSalesTax()
        {
            if (User.IsInRole(ds.Role_Admin))
            {
                var saleTaxes = db.SalesTax_Pts.ToList();

                List<Tuple<SalesTax_Pt, decimal, int, string>> obj = new List<Tuple<SalesTax_Pt, decimal, int, string>>();
                foreach (var item in saleTaxes)
                {
                    var invoice_no = db.Invoices.Where(m => m.SalePurchase_Id == item.STP_Id && m.Invoice_Type == ds.SalesTax_InvoiceType).SingleOrDefault().Invoice_No;
                    obj.Add(new Tuple<SalesTax_Pt, decimal, int, string>(item, item.STP_TotalAmount - item.STP_TotalReceived, invoice_no, item.STP_Date.ToShortDateString()));
                }
                return Json(obj, JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GenerateSalesTaxInvoice(FormCollection form)
        {
            if (ModelState.IsValid)
            {
                var js = new JavaScriptSerializer();
                try
                {
                    int stPt_Id = 0;

                    var stPt = JsonConvert.DeserializeObject<SalesTax_Pt>(form["SalesTax_PtObj"]);
                    stPt.Customer = db.Customers.Where(c => c.Customer_Id == stPt.CustomerId).SingleOrDefault();
                    stPt.STP_Date = DateTime.Now;
                    stPt.STP_Status = true;
                    db.SalesTax_Pts.Add(stPt);
                    db.SaveChanges();

                    stPt_Id = db.SalesTax_Pts.Max(s => s.STP_Id);

                    try
                    {
                        List<SalesTax_Ch> soCh_list = js.Deserialize<SalesTax_Ch[]>(form["SalesTax_ChList"]).ToList();
                        foreach (var soCh in soCh_list)
                        {
                            SalesTax_Ch saleTaxCh = new SalesTax_Ch();
                            saleTaxCh.STP_Id = stPt_Id;
                            saleTaxCh.SalesTax_Pt = db.SalesTax_Pts.Find(stPt_Id);
                            saleTaxCh.SOC_Id = soCh.SOC_Id;
                            var saleOrderChObj = db.SaleOrder_Ches.Find(soCh.SOC_Id);
                            saleTaxCh.SaleOrder_Ch = saleOrderChObj;
                            db.SalesTax_Ches.Add(saleTaxCh);
                            db.SaveChanges();

                            // convert sale order ch status from pending to complete
                            saleOrderChObj.SOC_SalesTaxStatus = ds.Status_Complete;
                            db.Entry(saleOrderChObj).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    catch (Exception e)
                    { }

                    SalePurchaseInvoiceType mSalePurchaseInvoiceType = new SalePurchaseInvoiceType();
                    Invoice invoice = new Invoice()
                    {
                        Invoice_No = js.Deserialize<int>(form["InvoiceNo"]),
                        Invoice_DocumentNo = js.Deserialize<int>(form["Invoice_DocumentNo"]),
                        Invoice_Type = ds.SalesTax_InvoiceType,
                        SalePurchase_Id = stPt_Id,
                        Invoice_Date = DateTime.Now,
                        Invoice_Status = true
                    };
                    db.Invoices.Add(invoice);
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
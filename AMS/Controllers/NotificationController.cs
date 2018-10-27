using AMS.Models;
using AMS.Models.HardCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS.Controllers
{
    public class NotificationController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private DefaultStrings ds = new DefaultStrings();

        // GET: Notification
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NotifyAll()
        {
            var obj = db.Notifications.ToList().OrderByDescending(m => m.Notification_Id);
            return View(obj);
        }

        public JsonResult AllNotifySeen()
        {
            var list = db.Notifications.Where(n => n.Notification_IsSeen == false).ToList();
            foreach (var item in list)
            {
                item.Notification_IsSeen = true;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckAndNotify()
        {
            try
            {
                string sessionId = Session["UserId"].ToString();
                if (User.IsInRole(ds.Role_Admin))
                {
                    var obj = db.Notifications.Where(n => n.Notification_IsSeen == false && n.Notification_ItemType == ds.Role_Admin).ToList();
                    if (obj != null)
                    {
                        return Json(obj, JsonRequestBehavior.AllowGet);
                    }
                }
                else if (User.IsInRole(ds.Role_Customer))
                {
                    var obj = db.Notifications.Where(n => n.Notification_IsSeen == false && n.Notification_ItemType == ds.Role_Customer && n.Id == sessionId).ToList();
                    if (obj != null)
                    {
                        return Json(obj, JsonRequestBehavior.AllowGet);
                    }
                }
                else if (User.IsInRole(ds.Role_Vendor))
                {
                    var obj = db.Notifications.Where(n => n.Notification_IsSeen == false && n.Notification_ItemType == ds.Role_Vendor && n.Id == sessionId).ToList();
                    if (obj != null)
                    {
                        return Json(obj, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {  }
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}
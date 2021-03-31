using FAXER.PORTAL.Common;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class AdminNotificationController : Controller
    {
        SNotification _notificationServices = null;

        public AdminNotificationController()
        {
            _notificationServices = new SNotification();
        }

        [HttpGet]
        public ActionResult Notification()
        {


            var result = (from c in _notificationServices.GetAllNotification(0, DB.NotificationFor.Admin).ToList()
                          select new NotificationViewModel()
                          {
                              Title = Enum.GetName(typeof(DB.Title), c.Title),
                              ReceiverName = c.Name,
                              Amount = c.Amount,
                              Message = c.Message,
                              Time = c.CreationDate.ToString(),
                              HourAgo = c.CreationDate.CalulateTimeAgo()
                          }).ToList();
            return View(result);

        }

        [HttpGet]
        public JsonResult GetAllNotification(int receiverId, DB.NotificationFor notificationFor)
        {
            CommonAllServices commonAllServices = new CommonAllServices();
            var unpaidData = commonAllServices.GetMoneyFexBankAccount();

            var notifications = (from c in _notificationServices.GetAllNotification(0, DB.NotificationFor.Admin).ToList()
                                 join d in unpaidData on c.NotificationKey equals d.PaymentReference
                                 select new NotificationViewModel()
                                 {
                                     Title = Enum.GetName(typeof(DB.Title), c.Title),
                                     ReceiverName = c.Name,
                                     Amount = c.Amount,
                                     Message = c.Message,
                                     Time = c.CreationDate.ToString(),
                                     HourAgo = c.CreationDate.CalulateTimeAgo(),
                                     NotificationId = c.Id
                                 }).ToList();

            TransactionErrorServices _transactionErrorServices = new TransactionErrorServices();
            var transactionErrors = (from c in _transactionErrorServices.TransactionErrors().Where(x => x.TransactionErrorStatus == DB.TransactionErrorStatus.ErrorState).ToList()
                                     select new NotificationViewModel()
                                     {
                                         Message = c.ReceiptNo,
                                         Title = "Transaction Error",
                                         HourAgo = c.Date.CalulateTimeAgo(),
                                         Time = c.Date.ToString(),
                                     }).ToList();
            var result = notifications.Concat(transactionErrors);
            return Json(new { result }, JsonRequestBehavior.AllowGet);

        }
        // GET: Admin/AdminNotification
        public ActionResult Index()
        {
            return View();
        }
    }
}
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Controllers
{
    public class KiiPayBusinessNotificationController : Controller
    {
        SNotification _notificationServices = null;
        public KiiPayBusinessNotificationController()
        {
            _notificationServices = new SNotification();
        }
        // GET: KiiPayBusiness/KiiPayBusinessNotification
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Notification() {

            int BusinessId = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId;

            var result = (from c in _notificationServices.GetAllNotification(BusinessId, DB.NotificationFor.KiiPayBusiness).ToList()
                          select new NotificationViewModel()
                          {
                              Title = Enum.GetName(typeof(DB.Title),c.Title),
                              ReceiverName = c.Name,
                              Amount = c.Amount,
                              Message = c.Message,
                              Time = c.CreationDate.ToString(),
                              HourAgo = c.CreationDate.CalulateTimeAgo()
                          }).ToList();
            return View(result);
        }
    }
}
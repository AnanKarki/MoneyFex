using FAXER.PORTAL.Common;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class SenderNotificationController : Controller
    {
        SNotification _notificationServices = null;
        // GET: SenderNotification
        public SenderNotificationController()
        {
            _notificationServices = new SNotification();
        }

        public ActionResult Notification()
        {
            int SenderId = Common.FaxerSession.LoggedUser.Id;

            var result = (from c in _notificationServices.GetAllNotification(SenderId, DB.NotificationFor.Sender).ToList()
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
    }
}
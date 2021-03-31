using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class NotificationController : Controller
    {
        SNotification _notificationServices = null;
        public NotificationController()
        {
            _notificationServices = new SNotification();
        }
        // GET: Notification
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetAllNotification(int ReceiverId , NotificationFor notificationFor ) {

            var result = (from c in _notificationServices.GetAllNotification(ReceiverId, notificationFor).OrderBy(x  => x.IsSeen).ToList()
                          select new NotificationViewModel() {
                              NotificationId = c.Id,
                              ReceiverName = c.Name,
                              Amount = c.Amount,
                              Message = c.Message,
                              Time = c.CreationDate.ToString(),
                              HourAgo = c.CreationDate.CalulateTimeAgo()
                          }).ToList();

            return Json(new 
            {
                Data = result,
                Count = _notificationServices.GetAllNotification(ReceiverId, notificationFor).Where(x => x.IsSeen == false).Count()

            }, JsonRequestBehavior.AllowGet);

        }


        [HttpGet]
        public JsonResult NoticationSeen(int NotificationId) {

            var data = _notificationServices.GetAllNotification().Where(x => x.Id == NotificationId).FirstOrDefault();

            if (data != null) {


                if (data.IsSeen == true)
                {

                    return Json(new
                    {
                        result = false

                    }, JsonRequestBehavior.AllowGet);
                }
                data.IsSeen = true;
                _notificationServices.UpdateNotificationToSeen(data);
            }

            return Json(new
            {
                result = true

            }, JsonRequestBehavior.AllowGet);

        }
        

    }
}
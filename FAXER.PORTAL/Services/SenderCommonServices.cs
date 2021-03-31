using FAXER.PORTAL.Common;
using FAXER.PORTAL.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SenderCommonServices
    {

        DB.FAXEREntities dbContext = null;

        public SenderCommonServices()
        {
            dbContext = new DB.FAXEREntities();
        }
        public void SendNotificationToSenderKiiPayPersonalAccount(DB.Notification notification)
        {

            SNotification _notificationServices = new SNotification();
            var result = _notificationServices.SaveNotification(notification);

            string hourago = result.CreationDate.CalulateTimeAgo();
            //HubController.SendToDashBoard(result.ReceiverId.ToString(), result.Name ,result.Message ,  result.Amount , result.CreationDate.ToString() , hourago , null);
            HubController.SendToKiiPayPersonal(result.ReceiverId.ToString(),  result.Name, result.Message, result.Amount, result.CreationDate.ToString(), hourago, null);

            var data = dbContext.SenderKiiPayPersonalAccount.Where(x => x.KiiPayPersonalWalletId == result.ReceiverId).FirstOrDefault();
            if (data != null)
            {
                var notificationForSender = new DB.Notification() {

                    SenderId = notification.SenderId,
                    ReceiverId = data.SenderId,
                    Amount = notification.Amount,
                    CreationDate = notification.CreationDate,
                    Title = notification.Title,
                    Message = notification.Message,
                    NotificationReceiver = DB.NotificationFor.Sender,
                    NotificationSender = notification.NotificationSender,
                    Name = notification.Name,
                };
                
                var result2 = _notificationServices.SaveNotification(notificationForSender);

                HubController.SendToSender(data.SenderId.ToString(), result2.Name, result2.Message, result2.Amount, result2.CreationDate.ToString(), hourago, null);
            }
        }


        public void SendNotification(DB.Notification notification)
        {

            SNotification _notificationServices = new SNotification();
            var result = _notificationServices.SaveNotification(notification);

            string hourago = result.CreationDate.CalulateTimeAgo();
            //HubController.SendToDashBoard(result.ReceiverId.ToString(), result.Name ,result.Message ,  result.Amount , result.CreationDate.ToString() , hourago , null);
            HubController.SendToSender(result.ReceiverId.ToString(), result.Name, result.Message, result.Amount, result.CreationDate.ToString(), hourago, null);

        }
        public void SendNotificationToAdmin(DB.Notification notification)
        {

            SNotification _notificationServices = new SNotification();
            var result = _notificationServices.SaveNotification(notification);

            string hourago = result.CreationDate.CalulateTimeAgo();
            //HubController.SendToDashBoard(result.ReceiverId.ToString(), result.Name ,result.Message ,  result.Amount , result.CreationDate.ToString() , hourago , null);
            HubController.sendToAdmin(result.ReceiverId.ToString(), result.Name, result.Message, result.Amount, result.CreationDate.ToString(), hourago, null);

        }

    }
}
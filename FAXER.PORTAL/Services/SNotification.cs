using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SNotification
    {
        DB.FAXEREntities dbContext = null;
        public SNotification()
        {
            dbContext = new DB.FAXEREntities();
        }

        public Notification SaveNotification(Notification notification)
        {

            dbContext.Notification.Add(notification);
            dbContext.SaveChanges();
            return notification;
        }

        public List<Notification> GetAllNotification()
        {

            return dbContext.Notification.ToList();
        }

        public List<Notification> GetAllNotification(int ReceiverId, NotificationFor notificationReceiver)
        {
            var data = dbContext.Notification.Where(x => x.NotificationReceiver == notificationReceiver && x.ReceiverId == ReceiverId && x.IsSeen == false).OrderByDescending(x => x.CreationDate).ToList();


            return data;
        }

        public Notification UpdateNotificationToSeen(Notification notification)
        {

            dbContext.Entry<Notification>(notification).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return notification;

        }
    }



}
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public interface IMobileNotificationManager
    {
        IQueryable<MobileNotification> MobileNotifications();
        MobileNotification MobileNotificationById(int id);
        void AddMobileNotification(MobileNotification mobileNotification);
        void UpdateMobileNotification(MobileNotification mobileNotification);
        void DeleteMobileNotification(MobileNotification mobileNotification);
        void SendNotificationToSender(MobileNotification mobileNotification);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.KiiPayBusiness
{
    public class KiiPayBusinessMobileNotificationViewModel
    {
        public int Id { get; set; }
        public int ReceiverId { get; set; }
        public int SenderId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }

        public string NotificationSender { get; set; }
        public string NotificationReceiver { get; set; }

        public bool IsSeen { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
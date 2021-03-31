using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class NotificationViewModel
    {

        public int NotificationId { get; set; }


        public string Title { get; set; }

        public string ReceiverName { get; set; }
        public string Message { get; set; }
        public string Amount { get; set; }
        public string Time { get; set; }
        public string HourAgo { get; set; }


    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.MoneyFex
{
    public class MobileNotificationViewModel
    {
        public int Id{ get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string CreatedTime { get; set; }
        //public int NotificationType { get; set; }
        public string CreationDate { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class MobileNotification
    {
        public int Id { get; set; }
        public string SendingCurrency { get; set; }
        public string SendingCountry { get; set; }
        public string ReceivingCurrency { get; set; }
        public string ReceivingCountry { get; set; }
        public int SenderId { get; set; }
        public CustomerTpyeAccToTime CustomerTpyeAccToTime { get; set; }
        public SendingNotificationMethod SendingNotificationMethod { get; set; }
        public NotificationType NotificationType { get; set; }
        public string NotificationHeading { get; set; }
        public string FullNotification { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public enum NotificationType
    {
        Select = 0,
        General = 1,
        RateAlert = 2,
    }
    public enum SendingNotificationMethod
    {
        Select = 0,
        MobileNotification = 1,
        SMS = 2,
        WhatsApp = 3
    }
    public enum CustomerTpyeAccToTime
    {
        All = 0,
        Week=1,
        TwoWeeks=2,
        ThreeWeeks=3,
        TwoMonths=4,
        ThreeMonths=5,
        SixMonths=6,
        Year=7,
        TwoYears=8,
    }
}
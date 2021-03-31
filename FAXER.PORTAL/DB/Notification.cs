using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class Notification
    {

        public int Id { get; set; }
        public int ReceiverId { get; set; }
        public int SenderId { get; set; }
        public Title Title { get; set; }
        public string Message { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }

        public NotificationFor NotificationSender { get; set; }
        public NotificationFor NotificationReceiver { get; set; }

        public bool IsSeen { get; set; }

        public string NotificationKey { get; set; }
        public DateTime CreationDate { get; set; }
        public NotificationType NotificationType { get; set; }
        public string NotificationTitle { get; set; }
    }

    public enum NotificationFor
    {

        KiiPayBusiness,
        kiiPayPersonal,
        Sender,
        Agent,
        Admin,
        Staff
    }
    public enum Title
    {

        BusinessLocalPayment,
        BusinessInternationalPayment,
        KiiPayPersonalLocalPayment,
        KiiPayPersonalInternationalPayment,
        InvoiceRequest,
        InvoicePaid,
        KiiPayWalletWithdrawal,
        MobileMoneyTransfer,
        CashPickUpTransfer
    }
}
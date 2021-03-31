using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class MobileNotifiactionViewModel
    {
        public const string BindProperty = "Id , SendingCurrency, SendingCountry, SendingCountryName, ReceivingCurrency,ReceivingCountry , ReceivingCountryName,SenderId ," +
                                           "SenderFullName, SendingNotificationMethod, SendingNotificationMethodName, NotificationType, NotificationTypeName, NotificationHeading," +
            " FullNotification, CreatedBy,CreatedByName ,CreatedDate ,CustomerTpyeAccToTime, CustomerTpyeAccToTimeName";
        public int Id { get; set; }
        [Required(ErrorMessage = "Select Currency")]
        public string SendingCurrency { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string SendingCountry { get; set; }
        public string SendingCountryName { get; set; }
        

        public string ReceivingCurrency { get; set; }
        
        public string ReceivingCountry { get; set; }
        public string ReceivingCountryName { get; set; }
        public int SenderId { get; set; }
        public string SenderAccount { get; set; }
        public string SenderFullName { get; set; }
        public SendingNotificationMethod SendingNotificationMethod { get; set; }
        public string SendingNotificationMethodName { get; set; }
        public NotificationType NotificationType { get; set; }
        public string NotificationTypeName { get; set; }
        [Required(ErrorMessage = "Enter Notification Heading")]
        public string NotificationHeading { get; set; }
        [Required(ErrorMessage = "Enter Meassage")]
        public string FullNotification { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public CustomerTpyeAccToTime CustomerTpyeAccToTime { get; set; }
        public string CustomerTpyeAccToTimeName { get; set; }
    }
    public class TransactionListForMobileNotificationVm
    {
        public List<BankAccountDeposit> BankAccountDeposits { get; set; }

        public List<MobileMoneyTransfer> MobileMoneyTransfers { get; set; }

        public List<FaxingNonCardTransaction> CashPickUpTransfers { get; set; }
    }
    public class NotifyUserSMSVm
    {
        public string binding_type { get; set; }
        public string Address { get; set; }
    }


    public enum Sms_binding_type { 

        sms,
        whatsapp
    
    }

}
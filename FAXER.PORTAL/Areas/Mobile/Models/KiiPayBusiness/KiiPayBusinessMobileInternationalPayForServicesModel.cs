using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.KiiPayBusiness
{
    public class KiiPayBusinessMobileInternationalPayForServicesModel
    {

        public const string BindProperty = "MobileNo ,ReceiverKiiPayBusinessId ,ReceiverKiiPayBusinessName ,ReceivingCountryCode , ReceivingCountryName , ReceivingCurrencyCode" +
             ", ReceivingPhoneCode , ReceivingCurrencySymbol ,SendingCurrencyCode ,SendingCountryCode , SendingAmount, SendingCurrencySymbol,ExchangeRate , Fee, TotalAmount" +
            ", SenderKiiPayBusinessId,PaymentReference , ReceivingAmount  ";
        public string MobileNo { get; set; }
        public int ReceiverKiiPayBusinessId { get; set; }
        public string ReceiverKiiPayBusinessName { get; set; }
        public string ReceivingCountryCode { get; set; }
        public string ReceivingCountryName { get; set; }
        public string ReceivingCurrencyCode { get; set; }
        public string ReceivingPhoneCode { get; set; }
        public string ReceivingCurrencySymbol { get; set; }
        public string SendingCurrencyCode { get; set; }
        public string SendingCountryCode { get; set; }
        public decimal SendingAmount { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal Fee { get; set; }
        public decimal TotalAmount { get; set; }
        public int SenderKiiPayBusinessId { get; set; }
        public string PaymentReference { get; set; }
        public decimal ReceivingAmount { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.KiiPayBusiness
{
    public class KiiPayBusinessMobileLocalPayForServicesModel
    {
        public const string BindProperty = "MobileNo ,ReceiverKiiPayBusinessId ,SenderKiiPayBusinessId ,ReceiverKiiPayBusinessName , SendingAmount , SendingCurrencyCode" +
             ", SendingCountryCode , SendingCurrencySymbol ,PaymentReference ,ExchangeRate , Fee, TotalAmount,ReceivingAmount";

        public string MobileNo { get; set; }
        public int ReceiverKiiPayBusinessId { get; set; }
        public int SenderKiiPayBusinessId { get; set; }
        public string ReceiverKiiPayBusinessName { get; set; }
        public decimal SendingAmount { get; set; }
        public string SendingCurrencyCode { get; set; }
        public string SendingCountryCode { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public string PaymentReference { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal Fee { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
    }
}
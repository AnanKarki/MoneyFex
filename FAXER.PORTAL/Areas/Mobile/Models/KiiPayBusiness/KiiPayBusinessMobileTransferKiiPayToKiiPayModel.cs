using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.KiiPayBusiness
{
    public class KiiPayBusinessMobileTransferKiiPayToKiiPayModel
    {
        public const string BindProperty = "MobileNo ,CurrencyCode ,ReciverId ,KiiPayBusinessId , Amount , Reference" +
               ", CurrencySymbol , CountryCode ,Fee ,ReceiverFullName ";
        public string MobileNo { get; set; }
        public string CurrencyCode { get; set; }
        public int ReciverId { get; set; }
        public int KiiPayBusinessId { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; }
        public string CurrencySymbol { get; set; }
        public string CountryCode { get; set; }
        public decimal Fee { get; set; }
        public string ReceiverFullName { get; set; }
    }
    public class KiiPayBusinessMobileAbroadTransferKiiPayToKiiPayModel
    {
        public const string BindProperty = "ReceiverMobileNo ,SenderCurrencyCode ,SenderCurrencySymbol ,KiiPayBusinessId , ReciverId , SendingAmount" +
               ", ReceivingAmount , Reference ,Fee ,ExchangeRate ,ReceiverCountryCode ,SenderCountryCode ,ReceiverCurrencyCode ,ReceiverCountryPhoneCode " +
            ", ReceiverCurrencySymbol,ReceiverFullName , TotalAmount  ";
        public string ReceiverMobileNo { get; set; }
        public string SenderCurrencyCode { get; set; }
        public string SenderCurrencySymbol { get; set; }
        public int KiiPayBusinessId { get; set; }
        public int ReciverId { get; set; }
        public decimal SendingAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public string Reference { get; set; }
        public decimal Fee { get; set; }
        public decimal ExchangeRate { get; set; }
        public string ReceiverCountryCode { get; set; }
        public string SenderCountryCode { get; set; }
        public string ReceiverCurrencyCode { get; set; }
        public string ReceiverCountryPhoneCode { get; set; }
        public string ReceiverCurrencySymbol { get; set; }
        public string ReceiverFullName { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class PayIntoAnotherWalletViewModel
    {
        public int Id { get; set; }
        public string SendingPhoneNumber { get; set; }
        public string ReceivingPhoneNumber { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public decimal SMSFee { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal PayingAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public string PaymentReference { get; set; }
        public PaymentType PaymentType { get; set; }
        public bool SendSMSNotification { get; set; }
        public string ReceiversName { get; internal set; }
        public string SendingCurrencySymbol { get; set; }
        public string ReceivingCurrencySymbol { get; set; }
        public string SendingCurrencyCode { get; set; }
        public string ReceivingCurrencyCode { get; set; }
        
        public string ReceivingCountryCode { get; set; }
    }
}
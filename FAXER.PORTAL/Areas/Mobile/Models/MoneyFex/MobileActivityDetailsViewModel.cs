using FAXER.PORTAL.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.MoneyFex
{
    public class MobileActivityDetailsViewModel
    {
        public int TranactionId { get; set; }
        public string ReceiptNo { get; set; }
        public string TransferMethodName { get; set; }
        public string StatusName { get; set; }
        public string ReceiverName { get; set; }
        public string ReceivingCity { get; set; }
        public string ReceivingCountry{ get; set; }
        public string ReceivingCountryCode{ get; set; }
        public string ReceivingCurrencyCode{ get; set; }
        public string SendingCurrencyCode{ get; set; }
        public string SendingCurrencySymbol{ get; set; }
        public decimal SendingAmount{ get; set; }
        public decimal Fee{ get; set; }
        public decimal TotalAmountPaid{ get; set; }
        public decimal ReceivingAmount{ get; set; }
        public string ReceiverFirstName{ get; set; }
        public decimal ExchangeRate{ get; set; }
        public string PaymentMethodName{ get; set; }
        public string Date{ get; set; }
        public string FormattedDate{ get; set; }
    }
}
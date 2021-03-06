using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class KiiPayRequestAPaymentSessionViewModel
    {
        public int Id { get; set; }
        public int SenderWalledId { get; set; }
        public int ReceiverWalletId { get; set; }
        public int SenderUserId { get; set; }
        public int ReceiverUserId { get; set; }
        public string ReceivingMobileNumber { get; set; }
        public string ReceivingCountryCode { get; set; }
        public string SendingCurrencyCode { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public string ReceivingCurrencyCode { get; set; }
        public string ReceivingCurrencySymbol { get; set; }
        public string ReceiverName { get; set; }
        public decimal SendingAmount { get; set; }
        public decimal PayingAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public string Note { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal Fee { get; set; }

    }
}
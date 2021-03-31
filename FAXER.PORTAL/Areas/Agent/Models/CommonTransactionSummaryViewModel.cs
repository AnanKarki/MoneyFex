using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class CommonTransactionSummaryViewModel
    {
        public const string BindProperty = "SendingAmount ,ReceivingAmount ,Fee , SendingCurrencySymbol, SendingCurrencyCode ,ReceivingCurrencySymbol ,ReceivingCurrecyCode" +
            ",ReceiverFirstName, TotalAmount, HasPaid";
        public decimal SendingAmount { get; set; }
        public decimal ReceivingAmount{ get; set; }
        public decimal Fee { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public string SendingCurrencyCode { get; set; }
        public string ReceivingCurrencySymbol { get; set; }
        public string ReceivingCurrecyCode { get; set; }
        public string ReceiverFirstName { get; set; }
        public decimal TotalAmount { get; set; }
        public bool HasPaid{ get; set; }
    }
}
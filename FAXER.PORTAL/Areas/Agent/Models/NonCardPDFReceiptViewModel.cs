using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class NonCardPDFReceiptViewModel
    {
        public string MFSReceiptNumber { get; set; }
        public DateTime TransactionDate { get; set; }
        public string FaxersFullName { get; set; }
        public string MFSCode { get; set; }
        public string ReceiversFullName { get; set; }
        public string Telephone { get; set; }
        public string AgentName { get; set; }
        public string AgentCode { get; set; }
        public string AmountSentRequested { get; set; }
        public string ExchangeRate { get; set; }
        public string Fee { get; set; }
        public string AmountReceived { get; set; }
        public string FaxingCountryCode { get; set; }
        public string ReceivingCountryCode { get; set; }
        public string ReceivingCurrencySymbol { get; set; }
        public string SendingCurrencySymbol { get; set; }
    }
}
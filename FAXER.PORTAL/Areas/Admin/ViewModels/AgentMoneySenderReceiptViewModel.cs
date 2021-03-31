using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class AgentMoneySenderReceiptViewModel
    {
        public int Id { get; set; }
        public string MFReceiptNumber { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionTime { get; set; }
        public string FaxerFullName { get; set; }
        public string MFCN { get; set; }
        public string ReceiverFullName { get; set; }
        public string Telephone { get; set; }
        public string AgentName { get; set; }
        public string AgentCode { get; set; }
        public string AmountSent { get; set; }
        public string ExchangeRate { get; set; }
        public string Fee { get; set; }
        public string AmountReceived { get; set; }
        public string TotalAmountSentAndFee { get; set; }
        
    }
}
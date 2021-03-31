using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class AgentRefundReceiptViewModel
    {
        public int Id { get; set; }
        public string ReceiptNumber { get; set; }
        public string TransactionReceiptNumber { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string SenderFullName { get; set; }
        public string MFCN { get; set; }
        public string ReceiverFullName { get; set; }
        public string Telephone { get; set; }
        public string RefundingAgentName { get; set; }
        public string RefundingAgentCode { get; set; }
        public string OrignalAmountSent { get; set; }
        public string RefundedAmount { get; set; }

    }
}
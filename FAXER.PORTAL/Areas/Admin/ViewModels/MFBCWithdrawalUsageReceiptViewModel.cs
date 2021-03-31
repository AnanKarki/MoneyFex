using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class MFBCWithdrawalUsageReceiptViewModel
    {
        public int Id { get; set; }
        public string MFReceiptNumber { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionTime { get; set; }
        public string BusinessMerchantName { get; set; }
        public string MFBCCardNumber { get; set; }
        public string BusinessCardUserFullName { get; set; }
        public string Telephone { get; set; }
        public string AgentName { get; set; }
        public string AgentCode { get; set; }
        public string AmountRequested { get; set; }
        public string ExchangeRate { get; set; }
        public string Fee { get; set; }
        public string AmountWithdrawn { get; set; }
        public string Currency { get; set; }
    }
}
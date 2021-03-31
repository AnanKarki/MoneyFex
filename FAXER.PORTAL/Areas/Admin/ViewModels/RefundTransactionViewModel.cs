using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class RefundTransactionViewModel
    {
        public string ReceiverName { get; set; }
        public string ReceiptNo { get; set; }
        public decimal RefundingAmount { get; set; }
        public RefundType RefundType { get; set; }
    }

    public enum RefundType
    {
        Select,
        Partial,
        FullRefund
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class AdminRefundReceiptViewModel
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
        public string RefundingAdminName { get; set; }
        public string RefundingAdminCode { get; set; }
        public string OrignalAmountSent { get; set; }
        public string RefundedAmount { get; set; }
        public string SendingCurrency { get; set; }
        public string ReceivingCurrency { get; set; }

        public string StaffLoginCode { get; set; }

    }
}
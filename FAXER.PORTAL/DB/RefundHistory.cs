using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class RefundHistory
    {
        public int Id { get; set; }
        public TransactionServiceType TransactionServiceType { get; set; }
        public int TransactionId { get; set; }
        public string ReceiptNo { get; set; }
        public decimal RefundedAmount { get; set; }
        public int RefundedBy { get; set; }
        public DateTime RefundedDate { get; set; }
        public string RefundReason { get; set; }
        public RefundType RefundType { get; set; }
    }
}
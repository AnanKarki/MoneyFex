using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class RefundOnDeletedMFTCCard
    {
        public int Id { get; set; }
        public string MFTCCardNumber { get; set; }
        public int FaxerId { get; set; }
        public decimal Amount { get; set; }
        public string DeletionReason { get; set; }
        public int RefundAdmin { get; set; }
        public DateTime RefundRequestDate { get; set; }
        public TimeSpan RefundRequestTime { get; set; }
        public string ReceiptNo { get; set; }
        public virtual FaxerInformation Faxer { get; set; }

    }
}
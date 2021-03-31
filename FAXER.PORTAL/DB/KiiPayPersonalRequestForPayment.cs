using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class KiiPayPersonalRequestForPayment
    {

        public int Id { get; set; }
        /// <summary>
        /// KiiPay Personal WalletId (Requested From)
        /// </summary>
        public int RequestSenderId { get; set; }
        /// <summary>
        /// KiiPay Personal WalletId (Requested To)
        /// </summary>
        public int RequestReceiverId { get; set; }

        public string RequestSendingCountry { get; set; }
        public string RequestReceivingCountry { get; set; }

        public decimal RequestSendingAmount { get; set; }
        public decimal RequestReceivingAmount { get; set; }
        /// <summary>
        /// 1 Requested To = 4 Requested From
        /// </summary>
        public decimal ExchangeRate { get; set; }

        public decimal Fee { get; set; }
        public decimal TotalAmount { get; set; }

        public string RequestNote { get; set; }
        public PaymentType RequestType { get; set; }

        public bool IsPaid { get; set; }
        public RequestPaymentStatus Status { get; set; }

        public DateTime RequestedDate { get; set; }
               
        public DateTime? PaymentDate { get; set; }

        public Module RequestFrom { get; set; }
        public Module RequestTo { get; set; }

        public bool IsDeleted { get; set; }
    }
    public enum RequestPaymentStatus
    {
        UnPaid,
        Paid,
        Cancelled,
        Deleted
    }
}
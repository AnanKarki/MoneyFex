using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderTransactionHistoryDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AvailableBalance { get; set; }
        public string MFCNNumber { get; set; }
        public RequestPaymentStatus Status { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string SendingAmount { get; set; }
        public string Fee { get; set; }
        public string AmountPaid { get; set; }
        public string ReceivingAmount { get; set; }
        public decimal ExchangeRate { get; set; }
        public string PaymentMethod { get; set; }
        public string Date { get; set; }
        public string SendingCurrencyCode { get; set; }
        public string ReceivingCurrencyCode { get; set; }
    }
}
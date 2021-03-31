using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ReceiverTransactionDetailsViewModel
    {
        public int TransactionId { get; set; }
        public string SendingCountry { get; set; }
        public string SendingCountryCode { get; set; }
        public string SendingCurrency { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public string ReceivingCountry { get; set; }
        public string ReceivingCountryCode { get; set; }
        public string ReceivingCurrency { get; set; }
        public string ReceivingCurrencySymbol { get; set; }
        public decimal SendingAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Fee { get; set; }
        public decimal ExchangeRate { get; set; }
        public string SenderName { get; set; }
        public string SenderFirstName { get; set; }
        public string SenderEmail { get; set; }
        public string SenderMobileNo { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverFirstName { get; set; }
        public string ReceiverMobileNo { get; set; }
        public string ReceiverAccountNo { get; set; }
        public string ReceiptNo { get; set; }
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string MobileProvider { get; set; }
        public string MFCN { get; set; }
        public Service Service { get; set; }
        public string ServiceName { get; set; }
        public string Status { get; set; }
        public string TransactionDate { get; set; }
        public string FormattedDate { get; set; }
        public string PaymentMethod { get; set; }
        public bool IsEuropeTransfer { get; set; }
    }
}
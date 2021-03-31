using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.MoneyFex
{
    public class TransactionDetailsViewModel
    {
        public int TransactionId { get; set; }
        public int SenderId { get; set; }
        public int RecipentId { get; set; }
        public Service Service { get; set; }
        public string ReceiverName { get; set; }
        public string ReciverFirstLetter { get; set; }
        public string SendingCountry { get; set; }
        public string SendingCountryCode { get; set; }
        public string SendingCountryCurrency { get; set; }
        public string SendingCountrySymbol { get; set; }
        public string ReceivingCountry { get; set; }
        public string ReceivingCountryCode { get; set; }
        public string ReceivingCountryCurrency { get; set; }
        public string ReceivingCountrySymbol { get; set; }
        public string City { get; set; }
        public decimal SendingAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Fee { get; set; }
        public decimal ExchangeRate { get; set; }
        public string TransactionDate { get; set; }
        public string FormattedTransactionDate { get; set; }
        
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public string AccountNo { get; set; }
        public string FormattedAccountNo { get; set; }

    }
}
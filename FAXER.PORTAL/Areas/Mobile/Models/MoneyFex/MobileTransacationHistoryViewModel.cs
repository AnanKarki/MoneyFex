using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.MoneyFex
{
    public class MobileTransacationHistoryViewModel
    {
        public List<MobileTransacationListvm> TransacationListvm { get; set; }
    }
    public class MobileTransacationListvm
    {
        public int TransactionId { get; set; }
        public int ReceipentId { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverFirstName { get; set; }
        public string ReceivingCurrencyCode { get; set; }
        public string SendingCurrencyCode { get; set; }
        public string ReceivingCountryName { get; set; }
        public string ReceivingTelephoneno { get; set; }
        public decimal SendingAmount { get; set; }
        public decimal ReceivingAmount { get; set; }

        public decimal ExchangeRateAmount { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Fee { get; set; }
        public decimal TotalAmount { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public string ReceivingCurrencySymbol { get; set; }
        public string AccountNumber { get; set; }
        public string ReceiptNumber { get; set; }
        public string ServiceName { get; set; }
        public Service Service { get; set; }
        public BankDepositStatus BankStatus { get; set; }
        public MobileMoneyTransferStatus MobileStatus { get; set; }
        public FaxingStatus CashPickUpStatus { get; set; }
        public string StatusName { get; set; }
        public string TransactionDate { get; set; }
        public string ReceivingCountryCode { get;  set; }
    }
}
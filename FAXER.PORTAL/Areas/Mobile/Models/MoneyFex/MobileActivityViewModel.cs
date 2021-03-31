using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.MoneyFex
{
    public class MobileActivityViewModel
    {

        public int TransactionId { get; set; }
        public string ReceiverName { get; set; }
        public string AccountNo { get; set; }
        public string ReceivingCountryCode { get; set; }
        public string ReceivingCurrencyCode { get; set; }
        public string ReecivingCurrencySymbol { get; set; }
        public TransactionTransferMethod TransferMethod { get; set; }
        public string TransferMethodName { get; set; }
        public decimal SendingAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public string Status { get; set; }
        public string StatusDescription { get; set; }
        public string ReceiptNo { get; set; }
        public string ReceivingCity { get; set; }
        public string ReceivingCountry { get; set; }
        public string SendingCurrencyCode { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public decimal Fee { get; set; }
        public decimal TotalAmountPaid { get; set; }
        public string ReceiverFirstName { get; set; }
        public decimal ExchangeRate { get; set; }
        public string PaymentMethodName { get; set; }
        public string FormattedDate { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime TransactionDateTime { get; set; }


        //latestpart
        public int ReceipentId { get; set; }
        public int BankId { get; set; }
        public string BankName { get; set; }
        public string BranchCode { get; set; }
        public int WalletId { get; set; }
        public string MobileNo { get; set; }
        public ReasonForTransfer Reason { get; set; }
        public string MobileWalletProviderName { get; set; }
        public string ReasonName { get; set; }
        public string ReceivingCountryPhoneCode { get; set; }
        public string SendingCountryCode { get; set; }
    }
    public class MobileActivityListvm
    {
        public string TransacationDate { get; set; }
        //public string MonthlyTransactionMeter { get; set; }
        public List<MobileActivityViewModel> ActivityListvm { get; set; }
    }
}
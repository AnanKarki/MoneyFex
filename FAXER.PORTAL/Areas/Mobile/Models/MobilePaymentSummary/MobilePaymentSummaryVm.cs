using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.MobilePaymentSummary
{
    public class MobilePaymentSummaryVm
    {
        public string SendingCountry { get; set; }
        public string ReceivingCountry { get; set; }
        public string SendingCurrencyCode { get; set; }
        public string ReceivingCurrencyCode { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public string ReceivingCurrencySymbol { get; set; }
        public string SendingFlagCode { get; set; }
        public string ReceivingFlagCode { get; set; }
        public decimal SendingAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Fee { get; set; }
        public bool IsIntroductoryRate { get; set; }
        public bool IsIntroductoryFee { get; set; }
        public decimal ActualFee { get; set; }

        public DB.TransactionTransferMethod TransferMethod { get; set; }

        public PaymentType PaymentType { get; set; }
        public string AccountNumber { get; set; }
        public string ShortCode { get; set; }
        public string LabelName { get; set; }
        public string PaymentReference { get; set; }
        public decimal BankFee { get; set; }


    }


    public class EnabledTransferMethodVm
    {

        public int TransferMethod { get; set; }
        public bool IsEnabled { get; set; }
    }

    public class PaymentSummaryArgument
    {

        public string SendingCountry { get; set; }
        public string SendingCurrency { get; set; }
        public string ReceivingCountry { get; set; }
        public string ReceivingCurrency { get; set; }
        public bool IsReceivingAmount { get; set; }
        public decimal SendingAmount { get; set; }

        public decimal ReceivingAmount { get; set; }
        public TransactionTransferMethod TransferMethod
        {
            get; set;
        }
        public int SenderId { get; set; }


    }
}
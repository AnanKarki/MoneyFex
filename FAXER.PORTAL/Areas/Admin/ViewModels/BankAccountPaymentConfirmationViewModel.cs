using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class BankAccountPaymentConfirmationViewModel
    {

        public const string BindProperty = " Id, PaymentReference,RecipetNo ,SenderName, SenderEmail, senderPhoneNo,senderDOB , ReceiverName,ReceiverEmail ,ReceiverPhoneNo ,ReciverCity" +
            " ,ReciverCountry , SenderCountry,ReceivingCurrency , SendingCurrency, SendingCurrencySymbol, sendingAmount, ReceivingAmount, Fee, ExchangeRate, TotalAmount, ReciverFirstname" +
            ",TranactionDate , MFCN, WalletProvider, BankName,BankCode ,AccountNumber , Status, IsPaid, Method, MoneyFexBankAccountLogId , SenderId";
        public int Id { get; set; }
        public string PaymentReference { get; set; }
        public string RecipetNo { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string senderPhoneNo { get; set; }
        public string senderDOB { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverEmail { get; set; }
        public string ReceiverPhoneNo { get; set; }
        public string ReciverCity { get; set; }
        public string ReciverCountry { get; set; }
        public string SenderCountry { get; set; }
        public string ReceivingCurrency { get; set; }
        public string SendingCurrency { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public decimal sendingAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public decimal Fee { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal TotalAmount { get; set; }
        public string ReciverFirstname { get; set; }
        public string TranactionDate { get; set; }
        public string MFCN { get; set; }
        public string WalletProvider { get; set; }
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string AccountNumber { get; set; }
        public string Status { get; set; }
        public bool IsPaid { get; set; }
        public TransactionTransferMethod Method { get; set; }
        public int MoneyFexBankAccountLogId { get; set; }
        public int SenderId { get; set; }
    }
}
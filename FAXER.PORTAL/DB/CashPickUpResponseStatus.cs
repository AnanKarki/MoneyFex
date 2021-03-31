using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class CashPickUpResponseStatus
    {
        public int Id { get; set; }
        public bool status { get; set; }
        public string message { get; set; }
        public int response { get; set; }
        public string extraResult { get; set; }
        public int TransactionId { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public virtual FaxingNonCardTransaction Transaction { get; set; }
    }
    public class CashPickUpTransactionResponseResult
    {
        public int Id { get; set; }
        public int CashPickUpResponseStatusId { get; set; }
        public decimal paymentAmount { get; set; }
        public string transactionReference { get; set; }
        public string partnerTransactionReference { get; set; }
        public string baseCurrencyCode { get; set; }
        public string targetCurrencyCode { get; set; }
        public decimal sourceAmount { get; set; }
        public string senderName { get; set; }
        public int transactionStatus { get; set; }
        public string transactionStatusDescription { get; set; }
        public string transactiondate { get; set; }
        public string transactioncharge { get; set; }
        public string payername { get; set; }
        public string errorDescription { get; set; }
        public string errorCode { get; set; }
        public string beneficiaryBankCode { get; set; }
        public string beneficiaryAccountNumber { get; set; }
        public string beneficiaryAccountName { get; set; }
        public decimal targetAmount { get; set; }
        public int retriedCount { get; set; }
        public string processorGateway { get; set; }
        public decimal amountInBaseCurrency { get; set; }
        public virtual CashPickUpResponseStatus CashPickUpResponseStatus { get; set; }
    }
}
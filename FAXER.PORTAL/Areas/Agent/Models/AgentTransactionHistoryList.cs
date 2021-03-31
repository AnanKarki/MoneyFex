using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class AgentTransactionHistoryList
    {
        public int Id { get; set; }
        public string SenderName { get; set; }
        public string ReceiptNumber { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionTime { get; set; }
        public int TransactionMonth { get; set; }

        /// <summary>
        /// If CashPickup is MFCN / If wallet payment Mobile NO
        /// </summary>
        /// 

        public string WalletName { get; set; }
        public string Status { get; set; }
        public string SenderNumber { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverEmail { get; set; }
        public string SenderEmail { get; set; }
        public string SenderDOB { get; set; }
        public string ReceiverNumber { get; set; }
        public string AgentName { get; set; }
        public string PayingStaffAgentName { get; set; }
        public string PayingStaffAccount{ get; set; }
        public string AgentNumber { get; set; }
        public string AgentAddress { get; set; }
        public string TransactionStaff { get; set; }
        public string SenderCountry { get; set; }
        public decimal AmountSent { get; set; }
        public decimal Fee { get; set; }
        public string SendingCurrency { get; set; }
        public string AgentCurrency { get; set; }
        public string ReceivingCurrrency { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public string ReceivingCurrencySymbol { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal ReceivingAmount { get; set; }
        public decimal ExchangeRate { get; set; }
        public string MFCN { get; set; }
        public  decimal AmountWithdrawal{get;set;}
        public string ReceiverDOB { get; set; }


        #region Bill Payment Details 
        public string AccountNumber { get; set; }

        #endregion
        public Type Type { get; set; }
        public string ReceiverCity { get; set; }
        public string ReceiverCountry { get; set; }
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string BankBranch { get; set; }
        public BankDepositStatus BankStatus{ get; set; }
        public bool IsRetryableCountry{ get; set; }
        public SystemCustomerType CustomerType { get; set; }
        public string CustomerTypeName { get; set; }
        public decimal AgentCommission { get; set; }
        public TransactionType TransactionType { get; set; }
    }

    public class AgentTransactionHistoryViewModel
    {
        public int Id { get; set; }
        public TransactionType FilterKey { get; set; }
        public Type FilterType { get; set; }
        public List<AgentTransactionHistoryList> TransactionHistoryList { get; set; }
    }
}
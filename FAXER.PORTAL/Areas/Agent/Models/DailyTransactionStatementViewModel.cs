using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class DailyTransactionStatementViewModel
    {
        public int Id { get; set; }
        public string ReferenceNumber { get; set; }
        public int Day { get; set; }

        public Month Month { get; set; }
        public int Year { get; set; }
        public TransactionType TransactionType { get; set; }
        public string AgentCurrencySymbol { get; set; }
        public decimal AccountBalance { get; set; }
        public bool IsPdf { get; set; }

        public decimal Commission { get; set; }

        public List<DailyTransactionStatementListVm> TransactionList { get; set; }
    }

    public class DailyTransactionStatementListVm
    {
        public int Id { get; set; }
        public TransactionType TransactionType { get; set; }
        public TransactionServiceType TransactionServiceType { get; set; }
        public string TransactionTypeName { get; set; }
        public string CurrencySymbol { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public string TransactionIdentifier { get; set; }
        public DateTime DateAndTime { get; set; }
        public string FormatedDate { get; set; }
        public string StaffName { get; set; }
        public string StatusName { get; set; }
        //receivingCountry
        public string ReceivingCountry { get; set; }
        public string ReceivingCountryName { get; set; }
        public string SendingCountry { get; set; }
        public string ReceiverName { get; set; }
        public decimal AgentCommission { get; set; }
        public Type Type { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public int AgentId { get; set; }
        public string TransferMethod { get; set; }
        public string AgentAccountNo { get; set; }
        public int TotalCount { get; set; }
        public int PayingStaffId { get; set; }
        public int RecipientId { get; set; }
        public string SendingCountryName { get; set; }
        public string SendingCurrency { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsAwaitForApproval { get; set; }

    }


    public enum TransactionType
    {
        [Display(Name = "All")]
        All = 0,
        [Display(Name = "Cash Pickup")]
        [Description("Cash Pickup")]
        CashPickUp = 1,

        [Display(Name = "KiiPay Wallet")]
        [Description("KiiPay Wallet")]
        KiiPayWallet = 2,
        [Display(Name = "Other Wallets Transfer")]
        [Description("Other Wallets Transfer")]
        OtherWalletTransfer = 3,
        [Display(Name = "Bank Account Deposit")]
        [Description("Bank Account Deposit")]
        BankAccountDeposit = 4,
        [Display(Name = "Pay Bills-Topup")]
        [Description("Pay Bills-Topup")]
        PayBillsTopUp = 5,
        [Display(Name = "Pay Bills-Monthly")]
        [Description("Pay Bills-Monthly")]
        PayBillsMonthly = 6,
        [Display(Name = "Refund")]
        [Description("Refund")]
        Refund = 7
    }


    public enum Type
    {
        Transfer = 1,
        Received = 2,
        Withdrawal = 3,
        BillPayment = 4
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class WalletStatementViewModel
    {
        public int Id { get; set; }
        public int Filter { get; set; }
        public List<WalletStatementList> WalletStatementList { get; set; }
    }

    public class WalletStatementList
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string AccountNumber { get; set; }
        public string Reference { get; set; }
        public string Gross { get; set; }
        public string Fee { get; set; }
        public string Net { get; set; }
        public decimal NetDecimal { get; set; }
        public string Balance { get; set; }
        public decimal BalanceDecimal { get; set; }
        public KiiPayPersonalWalletPaymentType PaymentType { get; set; }
        public bool IsRefunded { get; set; }
        public InOut InOut { get; set; }

    }

    public enum KiiPayPersonalWalletPaymentType
    {
        [Display(Name ="Service Payment")]
        [Description ("Service Payment")]
        PersonalToBusinessNational,

        [Display(Name = "Service Payment")]
        [Description("Service Payment")]
        PersonalToBusinessInternational,

        [Display(Name = "Kiipay Payment")]
        [Description("Kiipay Payment")]
        PersonalToPersonal,

        [Display(Name = "Kiipay Payment")]
        [Description("Kiipay Payment")]
        BusinessToPersonal,

        [Display(Name = "Credit Card Payment")]
        [Description("Credit Card Payment")]
        DebitCreditCard,

        [Display(Name = "Cash Pickup")]
        [Description("Cash Pickup")]
        CashPickUp,

        [Display(Name = "Bill Payment")]
        [Description("Bill Payment")]
        BillPayment,
        [Display(Name = "Mobile Transfer")]
        [Description("Mobile Transfer")]
        MobileTransfer,
        [Display(Name = "Bank Deposit")]
        [Description("Bank Deposit")]
        BankDeposit
    }
    public enum InOut
    {
        In,
        Out
    }
}
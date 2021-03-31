using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class SystemSummaryCreditViewModel
    {
        public const string BindProperty = "Id , TransactionHisotry, MonthlyAccountSummary ,PrefundingAmount ";
        public int Id { get; set; }

        public List<AgentTransactionHistoryList> TransactionHisotry { get; set; }
       
        public MonthlyAccountSummary MonthlyAccountSummary { get; set; }
        [Required(ErrorMessage ="Enter Amount")]
        public decimal PrefundingAmount { get; set; }
    }
    public enum SystemCustomerType
    {
        [Description("Customer Deposit")]
        CustomerDeposit,
        [Description("Customer Payment")]
        CustomerPayment,
        [Description("Prefunding")]
        Prefunding,
        [Description("MOneyFex Withdrawal")]
        MOneyFexWithdrawal,
    }
    public class MonthlyAccountSummary
    {
        public decimal CustomerDeposit { get; set; }
        public decimal CustomerPayment { get; set; }
        public decimal Fee { get; set; }
        public decimal Commission { get; set; }
        public decimal BankDeposit { get; set; }
        public decimal MoneyFexWithdrawal { get; set; }
        public decimal Total { get; set; }
    }
}
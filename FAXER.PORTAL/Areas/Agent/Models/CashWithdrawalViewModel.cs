using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class CashWithdrawalViewModel
    {
        public int Id { get; set; }
        public string NameOfAgent { get; set; }
        public string AgentAccountNumber { get; set; }
        public string AgentCountry { get; set; }
        public string AgentCity { get; set; }
        public decimal AccountBalance { get; set; }
        public CashWithdrawalType Withdrawal { get; set; }
        public Month Month { get; set; }
        public int Year { get; set; }
        public CashWithdrawalType TransactionType { get; set; }
        public string SearchText { get; set; }
        public List<WithdrawalListVm> WithdrawalList { get; set; }
    }

    public class WithdrawalListVm
    {
        public int Id { get; set; }
        public CashWithdrawalType WithdrawalType { get; set; }
        public decimal Amount { get; set; }
        public string StaffName { get; set; }
        public string StaffCode { get; set; }
        public DateTime DateAndTime { get; set; }
        public int TransactionMonth { get; set; }
        public string FormatedDateTime { get; set; }
        public WithdrawalStatus Status { get; set; }
        public string ReceiptUrl { get; set; }
        public string ReceiptNo { get; set; }
        public int AgentId { get; set; }
    }

    public enum CashWithdrawalType
    {
        [Display(Name = "Withdrawal By Agent")]
        WithdrawalByAgent = 1,
        [Display(Name = "MoneyFex Staff Withdrawal")]
        MoneyFexStaffWithdrawal = 2
    }

    public enum WithdrawalStatus
    {
        Pending,
        Confirmed
    }
}
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewAgentCashWithdrawalViewModel
    {
        public List<ViewAgentCashWithdrawalList> AgentCashWithdrawalList { get; set; }
        public IPagedList<ViewAgentCashWithdrawalList> AgentCashWithdrawalIPagedList { get; set; }
        public StaffMoreDetailsViewModel StaffDetails { get; set; }
        public int Day { get; set; }
        public Month Month { get; set; }
        public int Year { get; set; }
    }

    public class MoreWithdrawalDetails {
        public int Id{ get; set; }
        public int IsCashWithDrawal { get; set; }
        public string GeneratedDate { get; set; }
        public string GeneratedStaffName { get; set; }
        public string WithdrawalCode { get; set; }
    }
    public class ViewAgentCashWithdrawalList
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string AccountNo { get; set; }
        public string WithdrawalType { get; set; }
        public string NameOfStaffAgent { get; set; }
        public string StaffCode { get; set; }
        public decimal Amount { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public WithdrawalStatus Status { get; set; }
        public decimal AccountBalance { get; set; }
        public string ReceiptUrl { get; set; }
        public int IsWithdrawalByAgent { get; set; }
        public DateTime TransactionDate { get; set; }
        public string CurrencySymbol { get; set; }
    }

    public class StaffMoreDetailsViewModel {
        public int Id { get; set; }
        public string IDType { get; set; }
        public string IDNo { get; set; }
        public string ExpiryDate { get; set; }
        public string IssuingCountry { get; set; }
    }
}
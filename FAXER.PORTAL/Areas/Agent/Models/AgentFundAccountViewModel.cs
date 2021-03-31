using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class AgentFundAccountViewModel
    {
        public const string BindProperty = "Id ,AgentId,ApprovedBy , AgentName, Amount, BankAccountNo, City, BankSortCode, PaymentReference,AgentCountry,AgentCountryFlag" +
            ",AgentCode,AgentAccountNo,ResponsiblePerson,AgentCountryCurrency ,AgentCountryCurrencySymbol,Status , StatusName,Receipt,IsPaid";
        public int Id { get; set; }

        [Required(ErrorMessage = "Select Agent")]
        public int AgentId { get; set; }
        public int ApprovedBy { get; set; }
        public string AgentName { get; set; }
        [Required(ErrorMessage = "Enter Amount")]
        public decimal Amount { get; set; }
        public string BankAccountNo { get; set; }
        [Required(ErrorMessage = "Select City")]
        public string City { get; set; }
        public string BankSortCode { get; set; }
        public string PaymentReference { get; set; }

        [Required(ErrorMessage = "Select Country")]
        public string AgentCountry { get; set; }
        public string AgentCountryFlag { get; set; }
        public string AgentCode { get; set; }
        public string AgentAccountNo { get; set; }
        public string ResponsiblePerson { get; set; }
        public string AgentCountryCurrency { get; set; }
        public string AgentCountryCurrencySymbol { get; set; }
        public AgentFundStatus Status { get; set; }
        public string StatusName { get; set; }
        public string Receipt { get; set; }
        public SenderPaymentMode SenderPaymentMode { get; set; }
        public string SenderPaymentModeName { get; set; }
        public string FormattedCardNumber { get; set; }
        public DateTime DateTime { get; set; }
        public string Date { get; set; }
        public bool IsPaid { get; set; }
        public CardProcessorApi CardProcessorApi { get; set; }
    }
}
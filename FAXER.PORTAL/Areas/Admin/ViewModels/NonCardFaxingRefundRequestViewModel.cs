using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class NonCardFaxingRefundRequestViewModel
    {
        public const string BindProperty = "FaxerId ,FaxerFirstName ,FaxerMiddleName , FaxerLastName, FaxerAddress, FaxerCity ,FaxerCountryCode , FaxerCountry ,FaxerTelephone ,FaxerEmail ,StatusOfFax , StatusOfFaxName" +
            ",FaxerAccountNo ,TransactionDate ,TransactionId ,MFCNNumber , FaxReceiptNumber, ReceiptNumber,NameOfRefunder ,AdminCode, NameoFAgentRefunder, ReceiverId , ReceiverFirstName,ReceiverMiddleName ,ReceiverLastName" +
            ",ReceiverEmailAddress ,ReceiverCity ,ReceiverCountryCode,ReceiverCountry ,ReceiverTelephone,AgentId,NameOfAgency, AgencyMFSCode, ,PayingAgentName , YesConfirmed,NoConfirmed ,DateTime ,Time  ,FaxingAmount" +
            ",FaxingCurrency ,FaxingCurrencySymbol ,ExchangeRate ,ReceivedAmount , FaxingFee, RefundReason,RefundedDate ,RefendedTime , ReceivingCurrency, OperatingUserType";
        #region Faxer's Details
        public int FaxerId { get; set; }
        [Required(ErrorMessage = "The Field is required")]
        public string FaxerFirstName { get; set; }
        public string FaxerMiddleName { get; set; }
        [Required(ErrorMessage = "The Field is required")]
        public string FaxerLastName { get; set; }
        [Required(ErrorMessage = "The Field is required")]

        public string FaxerAddress { get; set; }
        [Required(ErrorMessage = "The Field is required")]

        public string FaxerCity { get; set; }
        [Required(ErrorMessage = "The Field is required")]
        public string FaxerCountryCode { get; set; }
        public string FaxerCountry { get; set; }
        [Required(ErrorMessage = "The Field is required")]

        public string FaxerTelephone { get; set; }
        [Required(ErrorMessage = "The Field is required")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter valid email")]
        public string FaxerEmail { get; set; }
        public FaxingStatus StatusOfFax { get; set; }
        [Required(ErrorMessage = "The Field is required")]

        public string StatusOfFaxName { get; set; }
        public string FaxerAccountNo { get; set; }
        public DateTime TransactionDate { get; set; }
        public int TransactionId { get; set; }
        public string MFCNNumber { get; set; }
        public string FaxReceiptNumber { get; set; }
        public string ReceiptNumber { get; set; }
        public string NameOfRefunder { get; set; }

        public string AdminCode { get; set; }

        public string NameoFAgentRefunder { get; set; }
        #endregion
        #region Receiver's Details
        public int ReceiverId { get; set; }
        [Required(ErrorMessage = "The Field is required")]

        public string ReceiverFirstName { get; set; }
        public string ReceiverMiddleName { get; set; }
        [Required(ErrorMessage = "The Field is required")]

        public string ReceiverLastName { get; set; }
        [Required(ErrorMessage = "The Field is required")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter valid email")]
        public string ReceiverEmailAddress { get; set; }
        [Required(ErrorMessage = "The Field is required")]

        public string ReceiverCity { get; set; }
        [Required(ErrorMessage = "The Field is required")]
        public string ReceiverCountryCode { get; set; }
        public string ReceiverCountry { get; set; }
        [Required(ErrorMessage = "The Field is required")]

        public string ReceiverTelephone { get; set; }
        #endregion
        #region Paying Agent
        public int AgentId { get; set; }
        public string NameOfAgency { get; set; }

        public string AgencyMFSCode { get; set; }
        public string PayingAgentName { get; set; }
        public bool YesConfirmed { get; set; }
        public bool NoConfirmed { get; set; }
        public DateTime? DateTime { get; set; }
        public string Time { get; set; }
        #endregion
        #region Faxing Amount and Exchange Rate
        [Required(ErrorMessage = "The Field is required")]

        public string FaxingAmount { get; set; }
        public string FaxingCurrency { get; set; }
        public string FaxingCurrencySymbol { get; set; }
        public string ExchangeRate { get; set; }
        public string ReceivedAmount { get; set; }

        [Required(ErrorMessage = "The Field is required")]

        public string FaxingFee { get; set; }

        #endregion
        #region Refund
        [Required(ErrorMessage = "The field is Required")]
        public string RefundReason { get; set; }

        public DateTime RefundedDate { get; set; }
        public string RefendedTime { get; set; }
        public string ReceivingCurrency { get; set; }

        #endregion

        
        public OperatingUserType OperatingUserType { get; set; }
    }
}
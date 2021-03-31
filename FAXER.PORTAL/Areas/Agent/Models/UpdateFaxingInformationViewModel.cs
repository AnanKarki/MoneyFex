using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class UpdateFaxingInformationViewModel
    {
        public const string BindProperty = "FaxerId , FaxerFirstName ,FaxerMiddleName,FaxerLastName , FaxerAddress ,FaxerCity,FaxerCountryCode , FaxerCountry ," +
            "FaxerTelephone,FaxerEmail , StatusOfFax,IdCardType,IdCardNumber , IdCardExpDate, StatusOfFaxName,  FaxerAccountNo,TransactionDate,TransactionId,MFCNNumber,FaxerCurrency," +
            "FaxerCurrencySymbol , ReceiverId,ReceiverFirstName, ReceiverMiddleName,ReceiverLastName,ReceiverEmailAddress ,ReceiverCity , ReceiverCountryCode ," +
          "ReceiverCountry  , ReceiverTelephone , ReceiverPhoneCode , ReceiverCurrency , ReceiverCurrencySymbol , AgentId,NameOfAgency ,AgencyMFSCode ,PayingAgentName ," +
            "NameofUpdatingAgent, IsConfirmed , DateTime ,Time, FaxingAmount , AmountToBeReceived , CurrentExchangeRate , FaxingFee , TotalAmountincludingFee , FaxingDetails";

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

        public string IdCardType { get; set; }
        [Required(ErrorMessage = "The Field is required")]

        public string IdCardNumber { get; set; }
        [Required(ErrorMessage = "The Field is required")]
      
        public System.DateTime IdCardExpDate { get; set; }
        public string StatusOfFaxName { get; set; }
        public string FaxerAccountNo { get; set; }
        public DateTime? TransactionDate { get; set; } 
        public int TransactionId { get; set; }
        public string MFCNNumber { get; set; }
        public string FaxerCurrency { get; set; }
        public string FaxerCurrencySymbol { get; set; }
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
        public string ReceiverPhoneCode { get; set; }
        public string ReceiverCurrency { get; set; }
        public string ReceiverCurrencySymbol { get; set; }
        #endregion
        #region Paying Agent
        public int AgentId { get; set; }
        [Required(ErrorMessage = "The Field is required")]

        public string NameOfAgency { get; set; }
        [Required(ErrorMessage = "The Field is required")]
        
        public string AgencyMFSCode { get; set; }                                                                                       
        public string PayingAgentName { get; set; }
        public string NameofUpdatingAgent { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime? DateTime { get; set; }
        public string Time { get; set; }
        #endregion
        #region Faxing Amount and Exchange Rate
        [Required(ErrorMessage = "The Field is required")]

        public string FaxingAmount { get; set; }
        

        public string AmountToBeReceived { get; set; }
        

        public string CurrentExchangeRate { get; set; }
        

        public string FaxingFee { get; set; }
       

        public string TotalAmountincludingFee { get; set; }
        public bool FaxingDetails { get; set; }
        #endregion
    }
}
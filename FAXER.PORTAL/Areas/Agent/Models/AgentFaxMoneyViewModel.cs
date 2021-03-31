using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class AgentFaxMoneyViewModel
    {
        public const string BindProperty = "FaxerId , FaxerFirstName ,FaxerMiddleName,FaxerLastName , FaxerDOB ,FaxerGender,FaxerAddress , FaxerCity ,FaxerCountry," +
             "FaxerCountryPhoneCode , FaxerTelephone ,FaxerCurrency,FaxerCurrencySymbol,AccountNo , FaxerEmail ,FaxerSearched,IdentificationTypeId,IdNumber,IdCardExpiringDate" +
            ",IssuingCountryCode,FaxingCountry,RecevingCountry,FaxedAmount,FaxingFee,CurrentExchangeRate,TotalAmountIncludingFee,RecevingAmount,StatusOfFax" +
            ",StatusOfFaxName ,FaxingDetails ,ExistingReceiver ,ReceiverSelected,ReceiverFirstName,ReceiverMiddleName,ReceiverLastName,ReceiverCurrency" +
            ",ReceiverCurrencySymbol,ReceiverAddress,ReceiverEmailAddress,ReceiverCity,ReceiverCountry,ReceiverCountryPhoneCode,ReceiverTelephone,AgentId,NameOfAgency , AgencyMFSCode" +
            ",DateTime ,PayingAgentName ,IsConfirmed ,NoExchangeRateSetup";
        #region Faxer's Details
        public int FaxerId { get; set; }
        [Required (ErrorMessage = "Please Enter First Name")]
        public string FaxerFirstName { get; set; }
        public string FaxerMiddleName { get; set; }
        [Required(ErrorMessage = "Please Enter last Name")]

        public string FaxerLastName { get; set; }
       
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please Enter Sender Date of Birth")]
        public DateTime? FaxerDOB { get; set; }
        public string FaxerGender { get; set; }
        [Required(ErrorMessage = "Please Enter Sender Address")]
        public string FaxerAddress { get; set; }
        [Required(ErrorMessage = "Please Enter Sender City")]

        public string FaxerCity { get; set; }
        [Required (ErrorMessage = "Please Select Sender Country")]
        public string FaxerCountry { get; set; }
        public string FaxerCountryPhoneCode { get; set; }
        public string FaxerTelephone { get; set; }
        public string FaxerCurrency { get; set; }
        public string FaxerCurrencySymbol { get; set; }
        public string AccountNo { get; set; }
        //[Required(ErrorMessage = "Please Enter Faxer Email")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter valid email")]
        public string FaxerEmail { get; set; }
        public bool FaxerSearched { get; set; }
        [Required(ErrorMessage = "Please Select  Identification Type")]
        public string IdentificationTypeId { get; set; }
        [Required(ErrorMessage = "Please Enter Id Number")]
        public string IdNumber { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please Enter Id Card Expiring Date")]
        public System.DateTime IdCardExpiringDate { get; set; }
        [Required(ErrorMessage = "Please Select IssingCountry")]
        public string IssuingCountryCode { get; set; }
        #endregion
        #region MFCN and Amount
        [Required(ErrorMessage = "Please Select FaxingCountry")]
        public string FaxingCountry { get; set; }
        [Required(ErrorMessage = "Please Select ReceivingCountry")]
        public string RecevingCountry { get; set; }
        [Required(ErrorMessage = "Please Enter Fax Amount")]
        public decimal FaxedAmount { get; set; }
        public string FaxingFee { get; set; }
        public string CurrentExchangeRate { get; set; }
        public string TotalAmountIncludingFee { get; set; }
        public decimal RecevingAmount { get; set; }
        public FaxingStatus StatusOfFax { get; set; }
        public string StatusOfFaxName { get; set; }
        public bool FaxingDetails { get; set; }
        #endregion
        #region Receiver's Details
        public string ExistingReceiver { get; set; }
        public bool ReceiverSelected { get; set; }
        
        public string ReceiverFirstName { get; set; }
        public string ReceiverMiddleName { get; set; }
        
        public string ReceiverLastName { get; set; }
        public string ReceiverCurrency { get; set; }
        public string ReceiverCurrencySymbol { get; set; }
        public string ReceiverAddress { get; set; }


        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter valid email")]
        public string ReceiverEmailAddress { get; set; }

        public string ReceiverCity { get; set; }
        
        public string ReceiverCountry { get; set; }
        public string ReceiverCountryPhoneCode { get; set; }
        public string ReceiverTelephone { get; set; }
        #endregion
        #region Paying Agent
        public int AgentId { get; set; }
        [Required(ErrorMessage = "Name of Agency is Required")]
        public string NameOfAgency { get; set; }
        [Required(ErrorMessage = "Agency MFS code is Required ")]
        public string AgencyMFSCode { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? DateTime { get; set; }
        
        public string PayingAgentName { get; set; }
        public bool IsConfirmed { get; set; }
        #endregion
        public bool NoExchangeRateSetup { get; set; }
    }
}
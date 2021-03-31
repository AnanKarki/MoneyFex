using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class PayNonMFTCCardUserViewModel
    {
        public const string BindProperty = "FaxerFirstName , FaxerMiddleName ,FaxerLastName,FaxerAddress , FaxerCity ,FaxerCountryCode,FaxerPhoneCode , FaxerCountry ," +
         "FaxerTelephone,FaxerEmail,StatusOfFax , StatusOfFaxName,DateTime,Time, FaxerCurrency ,  FaxerCurrencySymbol, ReceiverCurrency , ReceiverCurrencySymbol, " +
            " FaxedAmount, ReceivingAmount, MFCN, RefundRequest, ReceiverId, ReceiverFirstName, ReceiverMiddleName," +
         " ReceiverLastName, ReceiverAddress, ReceiverEmail, ReceiverCity ,ReceiverCountryCode,ReceiverCountry , ReceiverTelephone ,ReceiverPhoneCode,IdentificationTypeId , IdNumber ," +
         "IdCardExpiringDate,IssuingCountryCode , AgentId,NameOfAgency,AgencyMFSCode, PayingAgentName, IsConfirmed, WithdrawalPaymentOf";
        #region Faxer's Details
        public string FaxerFirstName { get; set; }
        public string FaxerMiddleName { get; set; }
        public string FaxerLastName { get; set; }
        public string FaxerAddress { get; set; }
        public string FaxerCity { get; set; }
        public string FaxerCountryCode { get; set; }
        public string FaxerPhoneCode { get; set; }
        public string FaxerCountry { get; set; }
        public string FaxerTelephone { get; set; }
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter valid email")]
        public string FaxerEmail { get; set; }
        public FaxingStatus StatusOfFax { get; set; }
        public string StatusOfFaxName { get; set; }
        public DateTime? DateTime { get; set; }
        public string Time { get; set; }
        public string FaxerCurrency { get; set; }
        public string FaxerCurrencySymbol { get; set; }

        public string ReceiverCurrency { get; set; }

        public string ReceiverCurrencySymbol { get; set; }
        #endregion
        #region MFCN and Amount
        public string FaxedAmount { get; set; }
        public string ReceivingAmount { get; set; }
        public string MFCN { get; set; }
        public string RefundRequest { get; set; }
        #endregion
        #region Receiver's Details
        public int ReceiverId { get; set; }
        [Required(ErrorMessage = "Please Enter Receiver First Name")]
        public string ReceiverFirstName { get; set; }
        public string ReceiverMiddleName { get; set; }
        [Required(ErrorMessage = "Please Enter Receiver Last Name")]
        public string ReceiverLastName { get; set; }
        public string ReceiverAddress { get; set; }
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter valid email")]
        public string ReceiverEmail { get; set; }
        public string ReceiverCity { get; set; }
        public string ReceiverCountryCode { get; set; }
        public string ReceiverCountry { get; set; }
        public string ReceiverTelephone { get; set; }
        public string ReceiverPhoneCode { get; set; }
        #endregion
        #region Receiver's Identification
        [Required(ErrorMessage = "Please Enter Identification Type")]
        public int IdentificationTypeId { get; set; }
        [Required(ErrorMessage = "Please Enter Id Number")]
        public string IdNumber { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please Enter Id Card Expiring Date")]
        public DateTime? IdCardExpiringDate { get; set; }
        [Required(ErrorMessage = "Please Enter Issuing Country")]
        public string IssuingCountryCode { get; set; }
        #endregion
        #region Paying Agent
        public int AgentId { get; set; }
        public string NameOfAgency { get; set; }
        public string AgencyMFSCode { get; set; }
        public string PayingAgentName { get; set; }
        public bool IsConfirmed { get; set; }
        #endregion

        public WithdrawalPaymentOf WithdrawalPaymentOf { get; set; }
    }
    public class PayMFTCCardUserViewModel
    {
        public const string BindProperty = "FaxerFirstName , FaxerMiddleName ,FaxerLastName,FaxerAddress , FaxerCity ,FaxerCountry,FaxerPhoneCode , FaxerTelephone ," +
           "FaxerEmail,StatusOfFax , StatusOfFaxName,DateTime,Time, MFTCCardId, MFTCCardName, MFTCCardUserEmail, MFTCCardNumber, AmountOnCard, WithDrawlLimit, LimitTypeEnum," +
           " LimitType, AmountRequested, TemporalEmailOrSMS, MFTCFirstName ,MFTCMiddleName,MFTCLastName , MFTCAddress ,MFTCCity,MFTCCountry , MFTCTelephone ," +
           "MFTCCardPhoneCode,MFTCCardURL , MFTCCardCurrency,MFTCCardCurrencySymbol,CardStatus, CardStatusEnum, IdentificationTypeId, IdNumber, IdCardExpiringDate, " +
            "IssuingCountryCode, AgentId, NameOfAgency, AgencyMFSCode, PayingAgentName, IsConfirmed";

        #region Faxer's Details
        public string FaxerFirstName { get; set; }
        public string FaxerMiddleName { get; set; }
        public string FaxerLastName { get; set; }
        public string FaxerAddress { get; set; }
        public string FaxerCity { get; set; }
        public string FaxerCountry { get; set; }

        public string FaxerPhoneCode { get; set; }
        public string FaxerTelephone { get; set; }
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter valid email")]
        public string FaxerEmail { get; set; }
        public FaxingStatus StatusOfFax { get; set; }
        public string StatusOfFaxName { get; set; }
        public DateTime? DateTime { get; set; }
        public string Time { get; set; }
        #endregion
        #region MFTC Card Details
        public int MFTCCardId { get; set; }
        public string MFTCCardName { get; set; }
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter valid email")]
        public string MFTCCardUserEmail { get; set; }
        public string MFTCCardNumber { get; set; }
        public decimal AmountOnCard { get; set; }
        public decimal WithDrawlLimit { get; set; }

        public CardLimitType LimitTypeEnum { get; set; }

        public string LimitType { get; set; }
        public decimal AmountRequested { get; set; }
        public string TemporalEmailOrSMS { get; set; }
        #endregion
        #region MFTC Card User Details
        public string MFTCFirstName { get; set; }
        public string MFTCMiddleName { get; set; }
        public string MFTCLastName { get; set; }
        public string MFTCAddress { get; set; }
        public string MFTCCity { get; set; }
        public string MFTCCountry { get; set; }
        public string MFTCTelephone { get; set; }
        public string MFTCCardPhoneCode { get; set; }

        public string MFTCCardURL { get; set; }
        public string MFTCCardCurrency { get; set; }
        public string MFTCCardCurrencySymbol { get; set; }

        public string CardStatus { get; set; }

        public CardStatus CardStatusEnum { get; set; }

        #endregion
        #region Receiver Identification 
        public string IdentificationTypeId { get; set; }
        [Required(ErrorMessage = "Please Enter Id Number")]
        public string IdNumber { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please Enter Id Card Expiring Date")]
        public DateTime? IdCardExpiringDate { get; set; }
        [Required(ErrorMessage = "Please Enter Issuing Country")]
        public string IssuingCountryCode { get; set; }
        #endregion
        #region Paying Agent
        public int AgentId { get; set; }
        public string NameOfAgency { get; set; }
        public string AgencyMFSCode { get; set; }
        public string PayingAgentName { get; set; }
        public bool IsConfirmed { get; set; }
        #endregion
    }

    public class PayMFBCCardUserViewModel
    {
        public const string BindProperty = "KiiPayBusinessInformationId , BusinessName ,BusinessLicenseNumber,Address , City ,Country,CountryCode , Telephone ," +
        "BusinessEmail,DateTime , Time,MFBCCardId,MFBCCardName, MFBCCardUserEmail, MFBCCardNumber, AmountOnCard, AmountRequested, MFBCCurrency, MFBCCurrencySymbol, TemporalEmailOrSMS," +
        " MFBCFirstName, MFBCMiddleName, MFBCLastName, MFBCCarduserDOB ,MFBCAddress,MFBCCardUserGander , MFBCCity ,MFBCCountry,MFBCTelephone , MFBCCardPhoneCode ," +
        "MFBCCardURL,MFBCCardStatus , MFBCCardStatusEnum,IdentificationTypeId,IdNumber, IdCardExpiringDate, IssuingCountryCode, AgentId, NameOfAgency, " +
         "AgencyMFSCode, PayingAgentName, IsConfirmed";

        #region BusinessMerchant Details

        public int KiiPayBusinessInformationId { get; set; }

        public string BusinessName { get; set; }
        public string BusinessLicenseNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public string CountryCode { get; set; }
        public string Telephone { get; set; }
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter valid email")]
        public string BusinessEmail { get; set; }
        public DateTime? DateTime { get; set; }
        public string Time { get; set; }
        #endregion
        #region MFBC Card Details
        public int MFBCCardId { get; set; }
        public string MFBCCardName { get; set; }
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter valid email")]
        public string MFBCCardUserEmail { get; set; }
        public string MFBCCardNumber { get; set; }
        public decimal AmountOnCard { get; set; }
        public decimal AmountRequested { get; set; }
        public string MFBCCurrency { get; set; }
        public string MFBCCurrencySymbol { get; set; }
        public string TemporalEmailOrSMS { get; set; }
        #endregion
        #region MFBC Card User Details
        public string MFBCFirstName { get; set; }
        public string MFBCMiddleName { get; set; }
        public string MFBCLastName { get; set; }
        public string MFBCCarduserDOB { get; set; }
        public string MFBCAddress { get; set; }
        public string MFBCCardUserGander { get; set; }
        public string MFBCCity { get; set; }
        public string MFBCCountry { get; set; }
        public string MFBCTelephone { get; set; }

        public string MFBCCardPhoneCode { get; set; }
        public string MFBCCardURL { get; set; }

        public string MFBCCardStatus { get; set; }

        public CardStatus MFBCCardStatusEnum { get; set; }

        #endregion
        #region Receiver Identification 
        public int IdentificationTypeId { get; set; }
        [Required(ErrorMessage = "Please Enter Id Number")]
        public string IdNumber { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please Enter Id Card Expiring Date")]
        public DateTime IdCardExpiringDate { get; set; }
        [Required(ErrorMessage = "Please Enter Issuing Country")]
        public string IssuingCountryCode { get; set; }
        #endregion
        #region Paying Agent
        public int AgentId { get; set; }
        public string NameOfAgency { get; set; }
        public string AgencyMFSCode { get; set; }
        public string PayingAgentName { get; set; }
        public bool IsConfirmed { get; set; }
        #endregion

    }


    public enum WithdrawalPaymentOf {

        Sender ,
        CardUser ,
        Merchant,
    }
}
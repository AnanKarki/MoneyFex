using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class LargeFundMoneyTransferFormVM
    {
        public const string BindProperty = "AmountInLocalCurrency , AmountInUSD ,DestinationCountryAndCity,ExpectedPaymentDate , SourceOfFund ,PurposeOfTransfer ,SenderFullName , SenderDateOfBirth ," +
         "SenderCountry,SenderGender , SenderAddress,SenderPhoneNo,SenderEmailAddress , SenderIdCardType, SenderIdNumber , SenderIdExpiryDate,SenderIdIssuingCountry ,SenderOccupation ,  " +
            "MoneyTransferCountInLastThreeMonth,SubmittedDate ,ReceiverName ,ReceiverTelephone , ReceiverAddress, ReceiverIdCardType ,ReceiverIdCardNumber ,ReceiverIdCardExpiryDate" +
            " ,ReceiverNationality , IsDocumentAttached , IsSenderAwareOfCharges , IsCheckedTheCountryRestriction ,RelationshipBetnSenderAndReceiver , IsMoneyFormAttached";
        #region Send Amount
        public decimal AmountInLocalCurrency { get; set; }
        public decimal AmountInUSD { get; set; }
        public string DestinationCountryAndCity { get; set; }
        public DateTime ExpectedPaymentDate { get; set; }
        public string SourceOfFund { get; set; }
        public string PurposeOfTransfer { get; set; }
        #endregion

        #region Sender Information 
        [Required(ErrorMessage = "Please enter the Full Name")]
        public string SenderFullName { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please enter the DOB")]
        public DateTime SenderDateOfBirth { get; set; }
        public string SenderCountry { get; set; }
        public Gender SenderGender { get; set; }
        [Required(ErrorMessage = "Please enter the address")]
        public string SenderAddress { get; set; }
        [Required(ErrorMessage = "Please enter the phone no")]
        public string SenderPhoneNo { get; set; }
        [Required(ErrorMessage = "Please enter the email address")]
        public string SenderEmailAddress { get; set; }
        [Required(ErrorMessage = "Please enter the id card type")]
        public string SenderIdCardType { get; set; }
        [Required(ErrorMessage = "Please enter the id card Number")]
        public string SenderIdNumber { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please enter the id card expiry date")]
        public DateTime SenderIdExpiryDate { get; set; }
        [Required(ErrorMessage = "Please enter the id card issuing country")]
        public string SenderIdIssuingCountry { get; set; }
        public string SenderOccupation { get; set; }
        public string MoneyTransferCountInLastThreeMonth { get; set; }
        public DateTime SubmittedDate { get; set; }

        #endregion

        #region Receiver Information
        [Required(ErrorMessage = "Please enter the receiver name")]
        public string ReceiverName { get; set; }
        [Required(ErrorMessage = "Please enter the receiver telephone")]
        public string ReceiverTelephone { get; set; }
        [Required(ErrorMessage = "Please enter the receiver address")]
        public string ReceiverAddress { get; set; }
        [Required(ErrorMessage = "Please enter the id card type")]
        public string ReceiverIdCardType { get; set; }
        [Required(ErrorMessage = "Please enter the id card number")]
        public string ReceiverIdCardNumber { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please enter the id card expiry date")]
        public DateTime ReceiverIdCardExpiryDate { get; set; }

        [Required(ErrorMessage = "Please enter the nationality")]
        public string ReceiverNationality { get; set; }

        #endregion

        public bool IsDocumentAttached { get; set; }
        public bool IsSenderAwareOfCharges { get; set; }
        public bool IsCheckedTheCountryRestriction { get; set; }
        public string RelationshipBetnSenderAndReceiver { get; set; }
        public bool IsMoneyFormAttached { get; set; }


    }
}
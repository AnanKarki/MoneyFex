using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class SourceOfFundDeclarationVM
    {

        public const string BindProperty = "SenderFullName , SenderDateOfBirth ,SenderCountry,SenderGender , SenderAddress ,SenderPhoneNo,SenderEmailAddress , SenderIdCardType " +
                 ",SenderIdNumber,SenderIdExpiryDate , SenderIdIssuingCountry,SenderOccupation,SubmittedDate , ReceiverName, ReceiverTelephone" +
            ",ReceiverCity , ReceverCountry, ReceiverEmail,RelationShipToSender ,OneOffAmount , LinkedAmount ,TransferType , CashToCash, VirtualAccount, BusinessVirtualAccount " +
            ",IsConfirm, IDAvailable, ProofOfSourceOfIncome,BankStatementLegalLetter ,Others ,ProofOfPurpose , MeansOfVerification,Email ,FaceToFace ,AgentStaffName" +
            ",AgentCountry ,AgentStaffAccountNo , AgentLoginCode,AdminStaffName ,AdminStaffLoginCode";
        #region Sender Information 
        [Required(ErrorMessage = "Please enter the Full Name")]
        public string SenderFullName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
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
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please enter the id card expiry date")]
        public DateTime SenderIdExpiryDate { get; set; }
        [Required(ErrorMessage = "Please enter the id card issuing country")]
        public string SenderIdIssuingCountry { get; set; }
        public string SenderOccupation { get; set; }
        public DateTime SubmittedDate { get; set; }

        #endregion

        #region Receiver Information
        [Required(ErrorMessage = "Please enter the receiver name")]
        public string ReceiverName { get; set; }


        [Required(ErrorMessage = "Please enter the receiver telephone")]
        public string ReceiverTelephone { get; set; }

        public string ReceiverAddress { get; set; }


        [Required(ErrorMessage = "Please enter the receiver city")]
        public string ReceiverCity { get; set; }


        [Required(ErrorMessage = "Please enter the receiver country")]
        public string ReceverCountry { get; set; }

        [Required(ErrorMessage = "Please enter the receiver email")]
        public string ReceiverEmail { get; set; }


        public string RelationShipToSender { get; set; }


        #endregion


        #region Amount / Type /Purpose 

        public decimal OneOffAmount { get; set; }
        public decimal LinkedAmount { get; set; }
        public SourceOfFundTransferType TransferType { get; set; }

        public string CashToCash { get; set; }
        public string VirtualAccount { get; set; }
        public string BusinessVirtualAccount { get; set; }


        public bool IsConfirm { get; set; }

        #endregion




        #region Agent /Admin Verification 

        public bool IDAvailable { get; set; }
        public bool ProofOfSourceOfIncome { get; set; }
        public string BankStatementLegalLetter { get; set; }
        public string Others { get; set; }
        public string ProofOfPurpose { get; set; }
        public string MeansOfVerification { get; set; }
        public string Email { get; set; }
        public string FaceToFace { get; set; }

        #endregion
        #region Agent Information 


        public string AgentStaffName { get; set; }
        public string AgentCountry { get; set; }
        public string AgentStaffAccountNo { get; set; }
        public string AgentLoginCode { get; set; }

        #endregion


        #region Admin Information 

        public string AdminStaffName { get; set; }
        public string AdminStaffLoginCode { get; set; }


        #endregion
    }

    public enum SourceOfFundTransferType
    {

        Type1,
        Type2
    }
}
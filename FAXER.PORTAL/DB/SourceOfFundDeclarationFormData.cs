using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class SourceOfFundDeclarationFormData
    {

        public int Id { get; set; }

        #region Sender Information 

        public string SenderFullName { get; set; }

      
        public DateTime SenderDateOfBirth { get; set; }

        public string SenderCountry { get; set; }
        public Gender SenderGender { get; set; }

        public string SenderAddress { get; set; }
     
        public string SenderPhoneNo { get; set; }
     
        public string SenderEmailAddress { get; set; }
  
        public string SenderIdCardType { get; set; }
        
        public string SenderIdNumber { get; set; }


        public DateTime SenderIdExpiryDate { get; set; }

        public string SenderIdIssuingCountry { get; set; }
        public string SenderOccupation { get; set; }






        public DateTime SubmittedDate { get; set; }



        public DateTime? ApprovedDate { get; set; }
        public int ApprovedStaffId { get; set; }
        public string ApprovedStaffAccountNo { get; set; }


        public FormStatus FormAction { get; set; }
        #endregion

        #region Receiver Information
  
        public string ReceiverName { get; set; }



        public string ReceiverTelephone { get; set; }
        
        public string ReceiverAddress { get; set; }


        public string ReceiverCity { get; set; }



        public string ReceverCountry { get; set; }


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

        public int AgentId { get; set; }
        public int AgentStaffId { get; set; }
        public string AgentStaffName { get; set; }
        public string AgentCountry { get; set; }
        public string AgentStaffAccountNo { get; set; }
        public string AgentLoginCode { get; set; }

        #endregion


        #region Admin Information 

        public string AdminStaffName { get; set; }
        public string AdminStaffLoginCode { get; set; }


        public int AdminStaffId { get; set; }

        #endregion
    }
}
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class LargeFundMoneyTransferFormData
    {
        public int Id { get; set; }
        public int AgentStaffId { get; set; }
        public int AgentId { get; set; }


        #region Send Amount

        public decimal AmountInLocalCurrency { get; set; }
        public decimal AmountInUSD { get; set; }


        public string DestinationCountryAndCity { get; set; }
        public DateTime ExpectedPaymentDate { get; set; }
        public string PurposeOfTransfer { get; set; }

        #endregion

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



        public int MoneyTransferCountInLastThreeMonth { get; set; }


        public DateTime SubmittedDate { get; set; }

        public DateTime? ApprovedDate { get; set; }
        public  int ApprovedStaffId { get; set; }
        public  string ApprovedStaffAccountNo { get; set; }
      
        public FormStatus FormAction { get; set; }
        #endregion

        #region Receiver Information

        public string ReceiverName { get; set; }

        public string ReceiverTelephone { get; set; }
        public string ReceiverAddress { get; set; }
        public string ReceiverIdCardType { get; set; }

        public string ReceiverIdCardNumber { get; set; }

        public DateTime ReceiverIdCardExpiryDate { get; set; }

        public string ReceiverNationality { get; set; }

        #endregion

        public bool IsDocumentAttached { get; set; }
        public bool IsSenderAwareOfCharges { get; set; }
        public bool IsCheckedTheCountryRestriction { get; set; }
        public string RelationshipBetnSenderAndReceiver { get; set; }
        public bool IsMoneyFormAttached { get; set; }


    }
}
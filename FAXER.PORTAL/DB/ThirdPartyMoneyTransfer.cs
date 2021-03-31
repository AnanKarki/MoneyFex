using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class ThirdPartyMoneyTransfer
    {

        public int Id { get; set; }

        #region Third Party Details

        public string SenderORBusinessName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Country { get; set; }
        public Gender Gender { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public string EmailAddress { get; set; }
        public string IdCardType { get; set; }
        public string IdNumber { get; set; }
        public DateTime IdExpiryDate { get; set; }
        public string IdIssuingCountry { get; set; }
        public string Occupation { get; set; }

        public bool IsThirdPartyTransfer { get; set; }

        public DateTime SubmittedDate { get; set; }



        #endregion

        #region AgentInformation

        public string AgentStaffCountry { get; set; }
        public string AgentStaffName { get; set; }
        public string AgentStaffAccountNo { get; set; }
        public string AgentStaffEmail { get; set; }
        public string AgentStaffPhoneNo { get; set; }

        public string AgentStaffLoginCode { get; set; }

        public int AgentStaffId { get; set; }
        public int AgentId { get; set; }
        #endregion

        #region MONEYFEX CONTROL NUMBER

        public decimal MainAmount { get; set; }
        public decimal Fee { get; set; }
        public string MFCN { get; set; }

        #endregion

        public bool IsApproved { get; set; }

        public bool IsDeclined { get; set; }
        #region Approved

        /// <summary>
        /// Admin staff Id
        /// </summary>
        public int ApprovedById { get; set; }
        public string ApprovedByName { get; set; }
        public string ApprovedStaffAccountNo { get; set; }
        public string AppovedTitle { get; set; }
        public DateTime? ApprovedDate { get; set; }
        #endregion
        #region Declined


        /// <summary>
        /// Admin staff Id
        /// </summary>
        public int DeclinedById { get; set; }
        public string DeclinedByName { get; set; }
        public string DeclinedTitle { get; set; }
        public DateTime? DeclinedDate { get; set; }

        #endregion

        public FormStatus FormAction { get; set; }

    }
}

public enum FormStatus
{
    Approved=1,
    Submitteded=2
}
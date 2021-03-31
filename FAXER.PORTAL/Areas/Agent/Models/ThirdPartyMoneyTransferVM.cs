using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class ThirdPartyMoneyTransferVM
    {
        public const string BindProperty = "SenderORBusinessName , DateOfBirth ,Country ,Gender , Address ,PhoneNo ,EmailAddress , IdCardType ," +
         "IdNumber,IdExpiryDate , IdIssuingCountry , Occupation ,IsThirdPartyTransfer , AgentStaffCountry, AgentStaffName , AgentStaffAccountNo, AgentStaffEmail , AgentStaffPhoneNo," +
            "AgentStaffLoginCode , AgentStaffId , AgentId , MainAmount, Fee, MFCN,IsApproved ,IsDeclined ,ApprovedByName , AppovedTitle,ApprovedDate , DeclinedByName, DeclinedTitle ,DeclinedDate ";

        #region Third Party Details

        [Required(ErrorMessage ="Please enter sender name or business name")]
        public string SenderORBusinessName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please enter the date of birth")]
        public DateTime DateOfBirth { get; set; }
        [Required(ErrorMessage = "Please select Country")]
        public string Country { get; set; }

        public Gender Gender { get; set; }
        [Required(ErrorMessage = "Please enter your address")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Please enter phone no")]
        public string PhoneNo { get; set; }
        [Required(ErrorMessage = "Please enter email address")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Please enter id card type")]
        public string IdCardType { get; set; }

        [Required(ErrorMessage = "Please enter id card number")]
        public string IdNumber { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please enter id card expiry date")]
        public DateTime IdExpiryDate { get; set; }

        [Required(ErrorMessage = "Please select id card issuing Country")]
        public string IdIssuingCountry { get; set; }
        public string Occupation { get; set; }

        public bool IsThirdPartyTransfer { get; set; }


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
        public string ApprovedByName { get; set; }
        public string AppovedTitle { get; set; }
        public DateTime? ApprovedDate { get; set; }
        #endregion
        #region Declined


        /// <summary>
        /// Admin staff Id
        /// </summary>
        public string DeclinedByName { get; set; }
        public string DeclinedTitle { get; set; }
        public DateTime? DeclinedDate { get; set; }



        #endregion

    }
}
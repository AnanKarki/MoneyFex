using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class SARFormVM
    {
        public const string BindProperty = "AgentId , AgentAccountNo ,AgentStaffLoginCode ,AgentStaffName , SubmittedDate ,SARTransactionType,MFCN , VirtualAccountNo ," +
              "BusinessVirtualAccountNo,TransactionDate , TransactionTime ,FirstName,LastName , DateOfBirth, IdType,IdNumber, " +
             "IdentificationExpiryDate,Address , IsSAR ,IsNonSAR,InvestigationDate , StaffName, StaffLoginCode, StaffAccountNo" +
             ", ReasonsForSuspicion, OtherSuspiciousReason";

        public int AgentId { get; set; }
        public string AgentAccountNo { get; set; }
        /// <summary>
        /// Agent staff LoginCode
        /// </summary>
        public string AgentStaffLoginCode { get; set; }
        public string AgentStaffName { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Enter Submitted Date of Birth")]
        public DateTime SubmittedDate { get; set; }

        public SARTransactionType SARTransactionType { get; set; }

        public string MFCN { get; set; }
        public string VirtualAccountNo { get; set; }
        public string BusinessVirtualAccountNo { get; set; }

        public DateTime TransactionDate { get; set; }
        public string TransactionTime { get; set; }



        #region Customer Detials 

        [Required(ErrorMessage = "Customer First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Customer Last Name is required")]
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Id Card Type is required")]
        public string IdType { get; set; }

        [Required(ErrorMessage = "Customer Id Card Number is required")]
        public string IdNumber { get; set; }

        public DateTime IdentificationExpiryDate { get; set; }

        [Required(ErrorMessage = "Address First Name is required")]
        public string Address { get; set; }

        #endregion

        #region HeadOffice

        public bool IsSAR { get; set; }

        public bool IsNonSAR { get; set; }

        public DateTime? InvestigationDate { get; set; }
        public string StaffName { get; set; }
        public string StaffLoginCode { get; set; }
        public string StaffAccountNo { get; set; }

        #endregion


        public List<CheckBoxViewModel> ReasonsForSuspicion { get; set; }

        [StringLength(1000)]
        public string OtherSuspiciousReason { get; set; }





    }

}
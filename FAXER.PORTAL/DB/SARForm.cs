using FAXER.PORTAL.Areas.Agent.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    /// <summary>
    /// Suspicious Activity Report 
    /// </summary>
    public class SARForm
    {
        public int Id { get; set; }
        public int AgentId { get; set; }

        public string AgentAccountNo { get; set; }
        public string AgentStaffLoginCode { get; set; }
        public string AgentStaffName { get; set; }

        public int AgentStaffId { get; set; }

        public DateTime SubmittedDate { get; set; }

        public SARTransactionType SARTransactionType { get; set; }

        public string MFCN { get; set; }
        public string VirtualAccountNo { get; set; }
        public string BusinessVirtualAccountNo { get; set; }

        public DateTime TransactionDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public int ApprovedBy { get; set; }
        public string TransactionTime { get; set; }


        #region Customer Detials 

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string    IdType { get; set; }
        public string IdNumber { get; set; }
        public DateTime IdentificationExpiryDate { get; set; }
        public string Address { get; set; }

        #endregion

        #region HeadOffice

        public bool IsSAR { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? InvestigationDate { get; set; }
        public string StaffName { get; set; }
        public int StaffId { get; set; }
        public string StaffAccountNo { get; set; }

        #endregion

        [StringLength(1000)]
        public string OtherSuspiciousReason { get; set; }

        public FormStatus FormAction { get; set; }
    }

    public enum SARTransactionType {

        CashToCash,
        VirtualAccount,
        BusinessVirtualAccount

    }


    public class ReasonForSuspicion
    {

        public int Id { get; set; }
        public string Name { get; set; }

    }

    public class SARForm_ReasonForSuspicion {

        public int Id { get; set; }
        public int SARFormId { get; set; }
        public int ReasonForSuspicionId { get; set; }


    }
}
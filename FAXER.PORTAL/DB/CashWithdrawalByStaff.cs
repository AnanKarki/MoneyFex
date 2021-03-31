using FAXER.PORTAL.Areas.Agent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class CashWithdrawalByStaff
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public int AgentId { get; set; }
        public int StaffIdCardType { get; set; }
        public string StaffIdCardNumber { get; set; }
        public DateTime StaffIDCardExpiryDate { get; set; }
        public string StaffIDCardIssuingCountry { get; set; }
        public string CashWithdrawalCode { get; set; }

        public int AgentStaffId { get; set; }

        public string AgentStaffName { get; set; }
        public decimal WithdrawalAmount { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public WithdrawalStatus Status { get; set; }

        public int? ConfirmedBy { get; set; }
        public DateTime? ConfirmedDateTime { get; set; }

        public string ReceiptNo { get; set; }


        public virtual StaffInformation Staff { get; set; }
        public virtual AgentInformation Agent { get; set; }
        
    }
}
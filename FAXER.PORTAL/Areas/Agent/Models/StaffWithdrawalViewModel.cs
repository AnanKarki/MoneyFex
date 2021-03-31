using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class StaffWithdrawalViewModel
    {
        public const string BindProperty = "Id , StaffId ,StaffCode,FirstName , MiddleName ,LastName,IDType , IDNumber ," +
         "IDExpiryDate,IssuingCountry , Address,StaffMoneyFexCode,StaffImageUrl , AgentName, AgentAccountNo , AgentStaffName, WithdrawalAmount,ConfirmVerification";
        public int Id { get; set; }
        public int StaffId { get; set; }
        public string StaffCode { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int IDType { get; set; }
        public string IDNumber { get; set; }
        public DateTime IDExpiryDate { get; set; }
        public string IssuingCountry { get; set; }
        public string Address { get; set; }
        public string StaffMoneyFexCode { get; set; }
        public string StaffImageUrl { get; set; }
        public string AgentName { get; set; }
        public string AgentAccountNo { get; set; }
        public string AgentStaffName { get; set; }
        public decimal WithdrawalAmount { get; set; }
        public bool ConfirmVerification { get; set; }
    }
}
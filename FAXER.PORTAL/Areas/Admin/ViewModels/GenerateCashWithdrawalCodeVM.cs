using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class GenerateCashWithdrawalCodeVM
    {
        public const string BindProperty = "CountryCode , CountryName ,City ,AgentId , AgentName, AgentCode,StaffId , StaffName,StaffCode , WithdrawalCode,Status,StatusName";


        [Required]
        public string CountryCode { get; set; }
        public string CountryName { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public int AgentId { get; set; }
        public string AgentName { get; set; }
        public string AgentCode { get; set; }

        [Required]
        public int StaffId { get; set; }
        public string StaffName { get; set; }
        public string StaffCode { get; set; }

       
        public string WithdrawalCode { get; set; }
        public AgentWithdrawalCodeStatus Status { get; set; }

        public string StatusName { get; set; }
        public string GeneratedStaffName { get; set; }
        public string GeneratedDate { get; set; }
        public DateTime Date { get; set; }

    }
}
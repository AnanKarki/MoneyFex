using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class EnableManualBankDepositViewModel
    {
        public const string BindProperty = "Id , PayingCountry ,IsEnabled , CreatedById,CreatedDate ,Agent , AgentAccountNo, AgentAddress,MobileNo ,MobileCode ,AgentId ";

        public int? Id { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string PayingCountry { get; set; }
        public bool IsEnabled { get; set; }
        public int CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        [Required(ErrorMessage = "Select Agent")]
        public string Agent { get; set; }
        public string AgentAccountNo { get; set; }
        public string AgentAddress { get; set; }
        [Required(ErrorMessage = "Enter Mobile Number")]
        public string MobileNo { get; set; }
        public string MobileCode { get; set; }
        public int AgentId{ get; set; }
    }
}
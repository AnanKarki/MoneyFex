using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessBankAccountVm
    {
        public int BankAccountId { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string Status { get; set; }

    }

    public class BankAccountAddBankAccountVM
    {
        public const string BindProperty = " Country , AccountUserName ,AccountNumber , BankName , BranchCode,BranchName , Address, MobileCode";
        [Required(ErrorMessage = "Please enter the Country.")]
        public string Country { get; set; }
        [Required(ErrorMessage = "Please enter the AccountUserName.")]
        public string AccountUserName { get; set; }
        [Required(ErrorMessage = "Please enter the AccountNumber.")]
        public string AccountNumber { get; set; }
        // [Required(ErrorMessage = "Please enter the BankName.")]
        public string BankName { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string Address { get; set; }
        [Required(ErrorMessage = "Please enter the MobileCode.")]
        public string MobileCode { get; set; }
    }
}
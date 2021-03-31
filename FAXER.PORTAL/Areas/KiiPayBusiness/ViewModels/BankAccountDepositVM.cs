using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class BankAccountDepositVM
    {
        public const string BindProperty = "AccountOwnerName ,MobileNumber ,AccountNumber ,BankId , BankName,BranchName , BranchId,BranchCode ";
        [Required(ErrorMessage ="Please enter the owner name.")]
        public string AccountOwnerName { get; set; }

        [Required(ErrorMessage = "Please enter the mobile no.")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "Please enter the account no.")]
        public string AccountNumber { get; set; }

        public int BankId { get; set; }

        public string BankName { get; set; }

        public string BranchName { get; set; }

        public int BranchId { get; set; }


        [Required(ErrorMessage = "Please select the branch name.")]
        public string BranchCode { get; set; }

    }

    public class BankAccountDepositInternationalVM : BankAccountDepositVM
    {

        public new const string BindProperty = "AccountOwnerName ,MobileNumber ,AccountNumber ,BankId , BankName,BranchName , " +
            "BranchId,BranchCode,PayingToCountry ,CountryPhoneCode";
        [Required(ErrorMessage ="Please select the country")]
        public string PayingToCountry { get; set; }

        public string CountryPhoneCode { get; set; }

    }
    public class BankAccountDepositDropDownVM
    {

        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

    }

    public class RecentAccountsPaidDropDownVM {

        public string AccountNo { get; set; }
        public string AccountOwnerName { get; set; }

    }
}
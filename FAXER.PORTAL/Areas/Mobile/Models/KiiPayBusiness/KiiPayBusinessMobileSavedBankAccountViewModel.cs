using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.KiiPayBusiness
{
    public class KiiPayBusinessMobileSavedBankAccountViewModel
    {
        public const string BindProperty = "BankAccountId ,BankId ,BranchId ,BankName , AccountNumber , FormattedAccountNumber" +
              ", CountryCode , CountryName ,AccountOwnerName ,BankCode , BranchCode, BranchName,KiiPayBusinessId , Status";
        public int BankAccountId { get; set; }
        public int BankId { get; set; }
        public int BranchId { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string FormattedAccountNumber { get; set; }

        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string AccountOwnerName { get; set; }
        public string BankCode { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public int KiiPayBusinessId { get; set; }
        public string Status { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class BankAccountDepositViewModel
    {
        public const string BindProperty = "Id ,RecentAccountNumber ,AccountOwnerName ,MobileNumber , AccountNumber, BankId, Branch , BranchCode";
        public int Id { get; set; }
        public string RecentAccountNumber { get; set; }
        public string AccountOwnerName { get; set; }
        public string MobileNumber { get; set; }
        public string AccountNumber { get; set; }
        public int BankId { get; set; }
        public int Branch { get; set; }
        public string BranchCode { get; set; }
    }
}
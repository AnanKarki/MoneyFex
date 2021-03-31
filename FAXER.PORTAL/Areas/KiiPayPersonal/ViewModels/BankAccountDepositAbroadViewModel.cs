using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class BankAccountDepositAbroadViewModel
    {
        public const string BindProperty = "Id ,Country ,RecentAccountNumber ,AccountOwner , TelephoneNumber , BankName" +
            ", Branch , BranchCode ,AccountNumber";

        public int Id { get; set; }
        public string Country { get; set; }
        public string RecentAccountNumber { get; set; }
        public string AccountOwner { get; set; }
        public string TelephoneNumber { get; set; }
        public int BankName { get; set; }
        public int Branch { get; set; }
        public string BranchCode { get; set; }
        public string AccountNumber { get; set; }
    }
}
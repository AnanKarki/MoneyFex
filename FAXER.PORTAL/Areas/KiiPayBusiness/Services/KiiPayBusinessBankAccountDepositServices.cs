using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Services
{
    public class KiiPayBusinessBankAccountDepositServices
    {
        DB.FAXEREntities dbContext = null;
        public KiiPayBusinessBankAccountDepositServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public List<BankAccountDepositDropDownVM> GetBanks() {

            List<BankAccountDepositDropDownVM> banks = new List<BankAccountDepositDropDownVM>();
            return banks;
        }
        public List<BankAccountDepositDropDownVM> GetBankBranches()
        {

            List<BankAccountDepositDropDownVM> branches = new List<BankAccountDepositDropDownVM>();
            return branches;
        }

        public List<RecentAccountsPaidDropDownVM> GetRecentAccountsPaid() {

            List<RecentAccountsPaidDropDownVM> recentAccountsPaids = new List<RecentAccountsPaidDropDownVM>();
            return recentAccountsPaids;

        }
    }
}
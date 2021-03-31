using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SBankAndBranch
    {
        DB.FAXEREntities dbContext = null;
        public SBankAndBranch()
        {
            dbContext = new DB.FAXEREntities();
        }

        public List<Bank> GetBanks() {

            var data = dbContext.Bank.ToList();
            return data;
        }
        public List<BankBranch> GetBankBranches() {

            var data = dbContext.BankBranch.ToList();
            return data;
        }
    }
}
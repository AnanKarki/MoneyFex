using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Services
{
    public class KiiPayBusinessBankAccountServices
    {

        DB.FAXEREntities dbContext = null;
        public KiiPayBusinessBankAccountServices()
        {
            dbContext = new DB.FAXEREntities();
        }


        public List<KiiPayBusinessBankAccountVm> GetSavedPersonalBankAccount()
        {
            var result = (from c in dbContext.BankAccount.ToList()
                          select new ViewModels.KiiPayBusinessBankAccountVm()
                          {
                              AccountNumber = c.AccountNo,
                              BankAccountId = c.Id,
                              BankName = c.LabelName,
                              Status = c.LabelValue

                          }).ToList();
            return result;
        }
    }
}
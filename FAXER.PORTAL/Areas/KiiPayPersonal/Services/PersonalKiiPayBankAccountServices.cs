
using FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.Services
{
    public class PersonalKiiPayBankAccountServices
    {
        FAXEREntities dbContext = null;
        public PersonalKiiPayBankAccountServices()
        {
            dbContext = new FAXEREntities();
        }

        public bool addBankAccount(AddNewBankAccountViewModel model)
        {
            if (model != null)
            {
                var data = new DB.SavedBank()
                {
                    Country = model.CountryCode,
                    OwnerName = model.NameOfAccountOwner,
                    AccountNumber = model.AccountNumber,
                    BankName = "",
                    BranchCode = model.Branchcode,
                    BranchName = model.Branch,
                    Address = model.Address,
                    CreatedDate =DateTime.Now,
                    UserId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId,
                    UserType = Module.KiiPayPersonal
                };
                dbContext.SavedBank.Add(data);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public void deleteBank(int id)
        {
            if(id != 0)
            {
                var data = dbContext.SavedBank.Find(id);
                if(data != null)
                {
                    data.isDeleted = true;
                    dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }
            }
        }

        public List<SavedBanksViewModel> getSavedBanksList()
        {
            var result = (from c in dbContext.SavedBank.Where(x => x.isDeleted == false).ToList()
                          select new SavedBanksViewModel()
                          {
                              Id = c.Id,
                              AccountNumber = Common.Common.FormatSavedBankAccountNumber(c.AccountNumber),
                              BankName = c.BankName,
                              Status = ""
                          }).ToList();
            return result;
        }
    }
}
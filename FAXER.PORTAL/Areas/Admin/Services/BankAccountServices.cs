using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class BankAccountServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices CommonService = new CommonServices();

        public List<BankAccountViewModel> getList(string Country, string AccountNo, string LabelName, string LabelValue)
        {
            var bankAccounts = dbContext.BankAccount.Where(x => x.IsDeleted == false);
            if (!string.IsNullOrEmpty(Country))
            {
                Country = Country.Trim();
                bankAccounts = bankAccounts.Where(x => x.CountryCode == Country);
            }
            if (!string.IsNullOrEmpty(AccountNo))
            {
                AccountNo = AccountNo.Trim();
                bankAccounts = bankAccounts.Where(x => x.AccountNo.ToLower().Contains(AccountNo.ToLower()));
            }
            if (!string.IsNullOrEmpty(LabelName))
            {
                LabelName = LabelName.Trim();
                bankAccounts = bankAccounts.Where(x => x.LabelName.ToLower().Contains(LabelName.ToLower()));
            }
            if (!string.IsNullOrEmpty(LabelValue))
            {
                LabelValue = LabelValue.Trim();
                bankAccounts = bankAccounts.Where(x => x.LabelValue.ToLower().Contains(LabelValue.ToLower()));
            }
            var result = (from c in bankAccounts.ToList()
                          join country in dbContext.Country on c.CountryCode equals country.CountryCode
                          select new BankAccountViewModel()
                          {
                              Id = c.Id,
                              Country = country.CountryName,
                              AccountNo = c.AccountNo,
                              LabelName = c.LabelName,
                              LabelValue = c.LabelValue,
                              CountryFlag = c.CountryCode.ToLower(),
                              TransferType = c.TransferType,
                              TransferTypeName = Common.Common.GetEnumDescription(c.TransferType)
                          }).ToList();
            return result;
        }

        public bool saveData(AddNewBankAccountViewModel model)
        {
            if (model != null)
            {
                DB.BankAccount data = new DB.BankAccount()
                {
                    CountryCode = model.Country,
                    AccountNo = model.AccountNo,
                    LabelName = model.LabelName,
                    LabelValue = model.LabelValue,
                    CreatedBy = Common.StaffSession.LoggedStaff.StaffId,
                    CreatedDateTime = DateTime.Now,
                    IsDeleted = false,
                    TransferType = model.TransferType
                };
                dbContext.BankAccount.Add(data);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool updateData(AddNewBankAccountViewModel model)
        {
            if (model != null)
            {
                var data = dbContext.BankAccount.Where(x => x.Id == model.Id).FirstOrDefault();
                if (data != null)
                {
                    data.CountryCode = model.Country;
                    data.AccountNo = model.AccountNo;
                    data.LabelName = model.LabelName;
                    data.LabelValue = model.LabelValue;
                    data.ModifiedDate = DateTime.Now;
                    data.LastModifiedBy = Common.StaffSession.LoggedStaff.StaffId;
                    data.TransferType = model.TransferType;
                    dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
            }
            return false;
        }
        public AddNewBankAccountViewModel getInfo(int id)
        {
            if (id != 0)
            {
                var data = dbContext.BankAccount.Find(id);
                if (data != null)
                {
                    var vm = new AddNewBankAccountViewModel()
                    {
                        Id = data.Id,
                        Country = data.CountryCode,
                        AccountNo = data.AccountNo,
                        LabelName = data.LabelName,
                        LabelValue = data.LabelValue
                    };
                    return vm;
                }

            }
            return null;
        }

        public bool deleteBankAccount(int id)
        {
            if (id != 0)
            {
                var data = dbContext.BankAccount.Where(x => x.Id == id).FirstOrDefault();
                if (data != null)
                {
                    data.IsDeleted = true;
                    dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
            }
            return false;
        }
    }
}
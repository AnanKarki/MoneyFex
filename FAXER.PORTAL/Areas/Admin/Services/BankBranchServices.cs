using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class BankBranchServices
    {
        DB.FAXEREntities dbContext = null;
        public BankBranchServices()
        {
            dbContext = new DB.FAXEREntities();
        }


        public List<DB.BankBranch> BankBrachList()
        {
            var data = dbContext.BankBranch.ToList();
            return data;
        }

        public List<BankBranchViewModel> GetBranchbankList(int BankId = 0)
        {
            var data = BankBrachList();
            if (BankId != 0)
            {
                data = data.Where(x => x.BankId == BankId).ToList();
            }
            var result = (from c in data
                          select new BankBranchViewModel()
                          {
                              Id = c.Id,
                              BankId = c.BankId,
                              BankName = c.Bank.Name,
                              BranchAddress = c.BranchAddress,
                              BranchCode = c.BranchCode,
                              BranchName = c.BranchName,
                              Country = Common.Common.GetCountryName(c.Bank.CountryCode),


                          }).ToList();
            return result;
        }

        internal BankBranchViewModel GetBranchbank(int id)
        {
            var data = BankBrachList().Where(x => x.Id == id).ToList();
            var result = (from c in data
                          select new BankBranchViewModel()
                          {
                              Id = c.Id,
                              BankId = c.BankId,
                              BankName = c.Bank.Name,
                              BranchAddress = c.BranchAddress,
                              BranchCode = c.BranchCode,
                              BranchName = c.BranchName,
                              Country = c.Bank.CountryCode,


                          }).FirstOrDefault();
            return result;
        }

        internal void deleteBankBranch(int id)
        {
            var data = BankBrachList().Where(x => x.Id == id).FirstOrDefault();
            dbContext.BankBranch.Remove(data);
            dbContext.SaveChanges();
        }

        internal void UpdateBankBranch(BankBranchViewModel vm)
        {
            var data = BankBrachList().Where(x => x.Id == vm.Id).FirstOrDefault();
            data.BankId = vm.BankId;
            data.BranchAddress = vm.BranchAddress;
            data.BranchCode = vm.BranchCode;
            data.BranchName = vm.BranchName;
            dbContext.Entry<BankBranch>(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
        }

        internal void AddBankBranch(BankBranchViewModel vm)
        {

            BankBranch bankBranch = new BankBranch()
            {
                BankId = vm.BankId,
                BranchAddress = vm.BranchAddress,
                BranchCode = vm.BranchCode,
                BranchName = vm.BranchName,
            };

            dbContext.BankBranch.Add(bankBranch);
            dbContext.SaveChanges();
        }
    }
}